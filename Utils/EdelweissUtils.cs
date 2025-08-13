using System.Collections.Generic;
using System.Linq;

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
    }
}