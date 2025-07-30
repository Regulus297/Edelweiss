using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Utils
{
    /// <summary>
    /// A class that loads form JSON files into proper widget objects that can be loaded by Edelweiss
    /// </summary>
    public static class FormLoader
    {
        internal class FieldData
        {
            internal string name;
            internal string displayName;
            internal string type;
            internal int row, col, rowspan, colspan;
            internal JToken value;
            internal JObject extraData;
        }
        static Dictionary<string, JObject> formCache = [];
        public static JObject LoadForm(string key)
        {
            if (formCache.TryGetValue(key, out JObject cached))
                return cached;

            JObject formData = PluginLoader.RequestJObject(key);
            JObject formWidget = new();

            string formID = formData.Value<string>("formID");
            string formTitle = formData.Value<string>("formTitle");
            formWidget.Add("id", formID);
            formWidget.Add("windowTitle", formTitle);
            formWidget.Add("type", "QWidget");
            formWidget.Add("onsubmit", "@netcode('FORM_SUBMITTED')");

            JObject layout = new()
            {
                {"type", "QGridLayout"}
            };

            List<JObject> children = [];
            List<FieldData> fields = [];

            JObject fieldInformation = formData.Value<JObject>("fieldInfo");

            foreach (var item in formData.Value<JObject>("fields"))
            {
                fields.Add(new FieldData()
                {
                    name = item.Key,
                    displayName = CamelCaseToText(item.Key),
                    type = GetFieldType(item.Value),
                    value = item.Value
                });

                if (fieldInformation != null)
                {
                    if (fieldInformation.Value<JObject>(item.Key) != null)
                    {
                        fields[^1].extraData = fieldInformation.Value<JObject>(item.Key);
                        fields[^1].type = fields[^1].extraData.Value<string>("type") ?? fields[^1].type;
                    }
                }
            }

            int totalCols = 4;
            int totalRows = 0;

            if (formData.Value<JToken>("fieldLayout") != null)
            {
                AssignSpecificGridLayout(fields, formData.Value<JToken>("fieldLayout"), out totalCols, out totalRows);
            }
            else
            {
                AssignDefaultGridLayout(fields, out totalRows);
            }

            foreach (FieldData field in fields)
            {
                JObject label = new()
                {
                    {"type", "QLabel"},
                    {"text", field.displayName},
                    {"alignment", "@alignment('right', 'vCenter')"},
                    {"row", field.row},
                    {"col", field.col}
                };
                JObject widget = GetWidget(field);
                if (field.extraData != null)
                {
                    foreach (var item in field.extraData)
                    {
                        if (item.Key == "type")
                            continue;
                        widget.Add(item.Key, item.Value);
                    }
                    Console.WriteLine(widget.ToString());
                }
                children.Add(label);
                children.Add(widget);
            }

            JObject submitButton = new()
            {
                {"type", "QPushButton"},
                {"text", formData.Value<string>("submitText") ?? "Submit"},
                {"specialType", "submit"},
                {"row", totalRows},
                {"col", 0},
                {"colspan", totalCols}
            };
            children.Add(submitButton);

            layout.Add("children", JToken.FromObject(children));
            formWidget.Add("layout", layout);

            return formWidget;
        }

        private static string CamelCaseToText(string input)
        {
            string output = "";
            foreach (char c in input)
            {
                if (char.IsUpper(c) && char.IsLower(output[^1]))
                {
                    output += " ";
                }
                output += c;
            }
            return char.ToUpper(output[0]) + output.Substring(1);
        }

        private static string GetFieldType(JToken value)
        {
            return value.Type switch
            {
                JTokenType.Array => "list",
                JTokenType.Boolean => "bool",
                JTokenType.Integer => "int",
                JTokenType.Float => "float",
                _ => "str"
            };
        }

        private static void AssignDefaultGridLayout(List<FieldData> fields, out int totalRows)
        {
            int i = 0;
            foreach (FieldData field in fields)
            {
                field.col = 2 * (i % 2);
                field.row = i / 2;
                field.rowspan = 1;
                field.colspan = 2;
                i++;
            }
            totalRows = i / 2;
        }

        private static void AssignSpecificGridLayout(List<FieldData> fields, JToken layout, out int totalCols, out int totalRows)
        {
            totalRows = 0;
            List<int> widgetNumbers = [];
            foreach (var item in layout)
            {
                widgetNumbers.Add(item.ToObject<List<string>>().Count);
            }

            totalCols = 2 * LCM(widgetNumbers);
            int row = 0;
            foreach (var item in layout)
            {
                List<string> fieldNames = item.ToObject<List<string>>();
                int colsPerWidget = totalCols / fieldNames.Count;
                int col = 0;
                foreach (string field in fieldNames)
                {
                    FieldData found = fields.Find(f => f.name == field);
                    if (found == null)
                        continue;
                    found.col = colsPerWidget * col;
                    found.row = row;
                    totalRows = row + 1;
                    found.rowspan = 1;
                    found.colspan = colsPerWidget;

                    col++;
                }
                row++;
            }
        }

        private static int LCM(List<int> arr)
        {
            int lcm = arr[0];
            for (int i = 0; i < arr.Count; i++)
            {
                int gcd = GCD(lcm, arr[i]);
                lcm = lcm * arr[i] / gcd;
            }
            return lcm;
        }

        private static int GCD(int a, int b)
        {
            if (b == 0)
                return a;
            return GCD(b, a % b);
        }

        private static JObject GetWidget(FieldData data)
        {
            JObject widget = new();
            switch (data.type)
            {
                case "list":
                    widget = new() {
                        {"items", data.value},
                        {"type", "QComboBox"},
                        {"id", data.name}
                    };
                    break;
                case "int":
                    widget = new()
                    {
                        {"type", "QLineEdit"},
                        {"id", data.name},
                        {"default", data.value.ToString()},
                        {"dataType", "int"}
                    };
                    break;
                case "float":
                    widget = new()
                    {
                        {"type", "QLineEdit"},
                        {"id", data.name},
                        {"default", data.value.ToString()},
                        {"dataType", "float"}
                    };
                    break;
                case "bool":
                    widget = new()
                    {
                        {"type", "QCheckBox"},
                        {"id", data.name},
                        {"default", data.value}
                    };
                    break;
                default:
                    widget = new()
                    {
                        {"type", "QLineEdit"},
                        {"id", data.name},
                        {"default", data.value.ToString()}
                    };
                    break;
            }

            widget.Add("row", data.row);
            widget.Add("col", data.col + 1);
            widget.Add("rowspan", data.rowspan);
            widget.Add("colspan", data.colspan - 1);

            return widget;
        }
    }
}