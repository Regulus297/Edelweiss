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

        /// <summary>
        /// Converts a JSON form file into a widget object that can be sent to the frontend
        /// </summary>
        /// <param name="key">The resource key of the JSON file</param>
        /// <param name="defaults">The default values of the fields if any. If null, the defined values in the JSON file are used.</param>
        /// <returns></returns>
        public static JObject LoadForm(string key, JObject defaults = null)
        {
            if (formCache.TryGetValue(key, out JObject cached))
                return cached;

            return LoadForm(key, PluginLoader.RequestJObject(key), defaults);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="formData"></param>
        /// <param name="defaults"></param>
        /// <returns></returns>
        public static JObject LoadForm(string key, JObject formData, JObject defaults = null)
        {
            JObject formWidget = new();

            string plugin = key.Split(":")[0];

            string formID = formData.Value<string>("formID");
            string formTitle = Language.GetTextOrDefault($"{plugin}.{formID}.Name") ?? formID.CamelCaseToText();
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
                FieldData fieldData = new FieldData()
                {
                    name = item.Key,
                    displayName = Language.GetTextOrDefault($"{plugin}.{formID}.Fields.{item.Key}") ?? item.Key.CamelCaseToText(),
                    type = GetFieldType(item.Value),
                    value = item.Value
                };
                fields.Add(fieldData);

                if (fieldInformation != null)
                {
                    if (fieldInformation.Value<JObject>(item.Key) != null)
                    {
                        fieldData.extraData = fieldInformation.Value<JObject>(item.Key);
                        fieldData.type = fieldData.extraData.Value<string>("type") ?? fieldData.type;
                    }
                }
                if (defaults != null)
                {
                    if (fieldData.type != "list")
                    {
                        fieldData.value = defaults?.Value<JToken>(fieldData.name) ?? fieldData.value;
                    }
                    else
                    {
                        if (fieldData.extraData == null)
                        {
                            fieldData.extraData = new JObject();
                        }
                        fieldData.extraData["default"] = defaults?.Value<JToken>(fieldData.name);
                    }
                }
            }

            int totalCols = 4;
            int totalRows;
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
                }
                children.Add(label);
                children.Add(widget);
            }

            JObject submitButton = new()
            {
                {"type", "QPushButton"},
                {"text", Language.GetTextOrDefault($"{plugin}.{formID}.Submit") ?? "Submit"},
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

        

        private static string GetFieldType(JToken value)
        {
            return value.Type switch
            {
                JTokenType.Array or JTokenType.Object => "list",
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
            totalRows = (int)Math.Ceiling(i / 2f);
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
            JObject widget;
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