using System;
using System.Collections.Generic;
using System.Linq;
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

        public static string Formatted(this Exception e)
        {
            if (e is not ScriptRuntimeException s)
                return e.ToString();

            return $"{s.DecoratedMessage}: \n {s}";
        }
    }
}