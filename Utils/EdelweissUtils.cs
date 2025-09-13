using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Edelweiss.Plugins;
using MoonSharp.Interpreter;

namespace Edelweiss.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class EdelweissUtils
    {
        /// <summary>
        /// Converts a camel case or pascal case string into a display name in title case
        /// </summary>
        public static string CamelCaseToText(this string input)
        {
            string output = "";
            foreach (char c in input)
            {
                if (char.IsUpper(c) && (output == "" || char.IsLower(output[^1])))
                {
                    output += " ";
                }
                output += c;
            }
            return char.ToUpper(output[0]) + output.Substring(1);
        }

        /// <summary>
        /// Converts mouse coordinates into tile coordinates
        /// </summary>
        /// <param name="mouseX">The x-coordinate of the mouse</param>
        /// <param name="mouseY">The y-coordinate of the mouse</param>
        /// <returns>The tile coordinates</returns>
        public static (int, int) ToTileCoordinate(float mouseX, float mouseY)
        {
            int x = (int)(mouseX / 8);
            int y = (int)(mouseY / 8);
            x -= (mouseX < 0) ? 1 : 0;
            y -= (mouseY < 0) ? 1 : 0;
            return (x, y);
        }

        /// <summary>
        /// If the item is present in the list, it is removed. If it is not present, it is added
        /// </summary>
        public static void Toggle<T>(this List<T> list, T item)
        {
            if (list.Contains(item))
                list.Remove(item);
            else
                list.Add(item);
        }

        /// <summary>
        /// Converts a DynValue into a hex color string in ARGB format
        /// </summary>
        public static string Color(this DynValue color)
        {
            if (color.Type == DataType.Table)
            {
                int r = (int)(color.Table.Get(1).Number * 255);
                int g = (int)(color.Table.Get(2).Number * 255);
                int b = (int)(color.Table.Get(3).Number * 255);

                int a = 255;
                if (color.Table.Length == 5)
                {
                    a = (int)(color.Table.Get(4).Number * 255);
                }

                string hex = $"#{a:X2}{r:X2}{g:X2}{b:X2}";

                return hex;
            }
            else if (color.Type == DataType.String)
                return color.String.StartsWith("#") ? color.String : "#" + color.String;
            return "#ffffff";
        }

        /// <summary>
        /// Returns true if the inputted array of arguments matches the desired signature
        /// </summary>
        public static bool MatchesSignature(this DynValue[] args, params DataType[] signature)
        {
            if (args.Length != signature.Length)
                return false;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Type != signature[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// If the exception is a ScriptRuntimeException, formats it appropriately so it displays Lua line numbers.
        /// </summary>
        public static string Formatted(this Exception e)
        {
            if (e is not ScriptRuntimeException s)
                return e.ToString();

            return $"{s.DecoratedMessage}: \n {s}";
        }

        /// <summary>
        /// Returns the Pythagorean distance between two points.
        /// </summary>
        public static float Distance(this Point point, Point other)
        {
            return MathF.Sqrt((point.X - other.X) * (point.X - other.X) + (point.Y - other.Y) * (point.Y - other.Y));
        }

        public static object MakeTuple(out Type tupleType, params object[] items)
        {
            int length = items.Length;
            if (length > 7)
            {
                object innerTuple = MakeTuple(out Type innerType, items.ToList().Slice(7, length - 7).ToArray());
                tupleType = typeof(ValueTuple<,,,,,,,>);
                tupleType.MakeGenericType(items[0].GetType(), items[1].GetType(), items[2].GetType(), items[3].GetType(), items[4].GetType(), items[5].GetType(), items[6].GetType(), innerType);
                return Activator.CreateInstance(tupleType, items[0], items[1], items[2], items[3], items[4], items[5], items[6], innerTuple);
            }
            tupleType = Type.GetType($"System.ValueTuple`{length}");
            tupleType = tupleType.MakeGenericType(items.Select(i => i.GetType()).ToArray());
            return Activator.CreateInstance(tupleType, items);
        }

        public static object Unpack(this Table table)
        {
            List<object> items = [];
            foreach (DynValue value in table.Values)
            {
                if (value.Value() != null)
                    items.Add(value.Value());
            }
            return MakeTuple(out Type _, items.ToArray());
        }

        public static object CastTuple<T, U>(object tuple)
        {
            if (tuple is not ITuple t)
                return null;

            object[] items = new object[t.Length];
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i].GetType() == typeof(T))
                {
                    items[i] = Convert.ChangeType(t[i], typeof(U));
                }
            }
            return MakeTuple(out Type _, items);
        }

        public static object Value(this DynValue value)
        {
            return value.Type switch
            {
                DataType.Boolean => value.Boolean,
                DataType.Number => value.Number,
                DataType.Table => value.Table,
                DataType.String => value.String,
                _ => null
            };
        }

        public static List<PluginAsset> GetPluginAssetsFromDirectory(string directory)
        {
            List<PluginAsset> assets = [];

            foreach (string assetDir in Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly))
                assets.Add(assetDir);
            foreach (string zipPath in Directory.GetFiles(directory, "*.zip", SearchOption.TopDirectoryOnly))
                assets.Add(zipPath);

            return assets;
        }

        /// <summary>
        /// Gets the value of the given key from a table, casting it to the desired type and returning the default value if the key is not in the table.
        /// </summary>
        public static T Get<T>(this Table table, string key, T defaultValue = default)
        {
            try
            {
                return (T)table.Get(key)?.Value() ?? defaultValue;
            }
            catch (InvalidCastException)
            {
                return defaultValue;
            }
        }

        public static FileStream Stream(this ZipArchive zipArchive)
        {
            var field = typeof(ZipArchive).GetField("_archiveStream", BindingFlags.NonPublic | BindingFlags.Instance);
            return (FileStream)field?.GetValue(zipArchive);
        }

        public static string Name(this ZipArchive zipArchive) => zipArchive.Stream().Name;
    }
}