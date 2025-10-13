using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            {
                string c = color.String.TrimStart('#');
                if (c.Length == 3)
                    return "#ff" + c.Substring(0, 1) + c.Substring(0, 1) + c.Substring(1, 1) + c.Substring(1, 1) + c.Substring(2, 1) + c.Substring(2, 1);
                if (c.Length == 6)
                    return "#ff" + c;
                if (c.Length == 8)
                    return "#" + c;
            }
            return "#ffffffff";
        }

        /// <summary>
        /// Returns a table containing the given color
        /// </summary>
        /// <param name="script">The script the table belongs to</param>
        /// <param name="color">The hex code of the color in ARGB format</param>
        public static DynValue NewColor(this Script script, string color)
        {
            int a = int.Parse(color.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
            int r = int.Parse(color.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            int g = int.Parse(color.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
            int b = int.Parse(color.Substring(7, 2), System.Globalization.NumberStyles.HexNumber);
            return DynValue.NewTable(script, DynValue.NewNumber(r / 255f), DynValue.NewNumber(g / 255f), DynValue.NewNumber(b / 255f), DynValue.NewNumber(a / 255f));
        }

        /// <summary>
        /// Gets the hex color code for the given RGBA ranging from 0-1
        /// </summary>
        public static string GetColor(float r, float g, float b, float a = 1)
        {
            return DynValue.NewTable(new Script(), DynValue.NewNumber(r), DynValue.NewNumber(g), DynValue.NewNumber(b), DynValue.NewNumber(a)).Color();
        }


        /// <summary>
        /// Gets the hex color code for the given RGBA ranging from 0-255
        /// </summary>
        public static string GetColor(int r, int g, int b, int a = 255) => GetColor(r / 255f, g / 255f, b / 255f, a / 255f);

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

        /// <summary>
        /// Converts a list of objects into a ValueTuple and outputs the type
        /// </summary>
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

        /// <summary>
        /// Returns all members of a table in tuple form
        /// </summary>
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

        /// <summary>
        /// For every member of the input tuple with the type T, converts it to type U in the output tuple
        /// </summary>
        /// <typeparam name="T">The source type</typeparam>
        /// <typeparam name="U">The target type</typeparam>
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

        /// <summary>
        /// Gets the value of a DynValue if it is a value type (bool, number, table or string) otherwise null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets all the directories and zip files in a directory, in that order and converts them to plugin assets
        /// </summary>
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

        /// <summary>
        /// Gets the value of the given index from a table, casting it to the desired type and returning the default value if the index is not in the table.
        /// </summary>
        public static T Get<T>(this Table table, int key, T defaultValue = default)
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

        /// <summary>
        /// Exposes the private stream of a given zip archive
        /// </summary>
        public static FileStream Stream(this ZipArchive zipArchive)
        {
            var field = typeof(ZipArchive).GetField("_archiveStream", BindingFlags.NonPublic | BindingFlags.Instance);
            return (FileStream)field?.GetValue(zipArchive);
        }

        /// <summary>
        /// Returns the path to a given zip archive
        /// </summary>
        public static string Path(this ZipArchive zipArchive) => zipArchive.Stream().Name;

        /// <summary>
        /// Converts a point to a Lua table
        /// </summary>
        public static Table ToLuaTable(this Point p, Script script, bool keyed = true)
        {
            Table table = new Table(script);
            table[keyed ? "x" : 1] = p.X;
            table[keyed ? "y" : 2] = p.Y;
            return table;
        }

        /// <summary>
        /// Cycles through a list
        /// </summary>
        public static T Cycle<T>(this List<T> items, T current, int amount = 1)
        {
            int i = items.IndexOf(current);
            if (i == -1)
                return current;

            i += amount;
            if (i < 0)
            {
                i += items.Count * Math.Abs(i);
            }

            i %= items.Count;
            return items[i];
        }

        /// <summary>
        /// Cycles through an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="current"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static T Cycle<T>(this T[] items, T current, int amount = 1)
        {
            int i = Array.IndexOf(items, current);
            if (i == -1)
                return current;

            i += amount;

            i = i.Mod(items.Length);
            return items[i];
        }

        /// <summary>
        /// Takes the modulus of two numbers ensuring the result is positive
        /// </summary>
        public static int Mod(this int a, int b)
        {
            return ((a % b) + b) % b;
        }

        /// <summary>
        /// Gets a particular key from the metatable of the table
        /// </summary>
        public static DynValue GetFromMetatable(this Table table, string key)
        {
            return table.OwnerScript.Call(table.MetaTable.Get("__index"), table, key);
        }

        /// <summary>
        /// Sets the given key to the given value using metatable setters
        /// </summary>
        public static void SetToMetatable(this Table table, string key, object value)
        {
            table.OwnerScript.Call(table.MetaTable.Get("__newindex"), table, key, DynValue.FromObject(table.OwnerScript, value));
        }

        /// <summary>
        /// Gets the smallest rectangle that completely covers both input rectangles
        /// </summary>
        public static Rectangle Combine(this Rectangle a, Rectangle b)
        {
            if (a == default || b == default)
            {
                return a == default ? b : a;
            }
            int minX = int.Min(a.X, b.X);
            int minY = int.Min(a.Y, b.Y);
            int maxX = int.Max(a.X + a.Width, b.X + b.Width);
            int maxY = int.Max(a.Y + a.Height, b.Y + b.Height);
            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Returns the scaled version of the given rectangle
        /// </summary>
        public static Rectangle Scaled(this Rectangle a, float scaleX, float scaleY)
        {
            return new Rectangle((int)(a.X * scaleX), (int)(a.Y * scaleY), (int)(a.Width * scaleX), (int)(a.Height * scaleY));
        }

        /// <summary>
        /// Returns the smallest axis-aligned rectangle that covers the rotated rectangle
        /// </summary>
        public static Rectangle Rotated(this Rectangle a, float rotation)
        {
            Point topLeft = a.Location.Rotated(rotation);
            Point topRight = new Point(a.X + a.Width, a.Y).Rotated(rotation);
            Point bottomLeft = new Point(a.X, a.Y + a.Height).Rotated(rotation);
            Point bottomRight = new Point(a.X + a.Width, a.Y + a.Height).Rotated(rotation);

            List<Point> points = [topLeft, topRight, bottomLeft, bottomRight];
            int minX = int.MaxValue, minY = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue;
            foreach (Point point in points)
            {
                minX = int.Min(point.X, minX);
                minY = int.Min(point.Y, minY);
                maxX = int.Max(point.X, maxX);
                maxY = int.Max(point.Y, maxY);
            }
            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Rotates a point by a given amount using (0, 0) as the anchor
        /// </summary>
        public static Point Rotated(this Point point, float rotation)
        {
            return new Point((int)(point.X * MathF.Cos(rotation) - point.Y * MathF.Sin(rotation)), (int)(point.X * MathF.Sin(rotation) + point.Y * MathF.Cos(rotation)));
        }

        /// <summary>
        /// Translates the given rectangle
        /// </summary>
        public static Rectangle Translated(this Rectangle a, int x, int y)
        {
            return new Rectangle(a.X + x, a.Y + y, a.Width, a.Height);
        }

        /// <summary>
        /// Converts the given table into a rectangle
        /// </summary>
        public static Rectangle ToRectangle(this Table table)
        {
            return new Rectangle((int)(double)table["x"], (int)(double)table["y"], (int)(double)table["width"], (int)(double)table["height"]);
        }
    }
}