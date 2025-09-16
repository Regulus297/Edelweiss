using System;
using Edelweiss.Loenn;
using Edelweiss.Utils;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Drawables
{
    /// <summary>
    /// A class that represents a drawable rectangle.
    /// </summary>
    public class Rectangle() : Drawable, ILuaConvertible
    {
        /// <summary>
        /// 
        /// </summary>
        public int x = 0, y = 0;
        /// <summary>
        /// 
        /// </summary>
        public int width = 0, height = 0;

        /// <summary>
        /// The fill color for the rectangle
        /// </summary>
        public string color = "#ffffff";

        /// <summary>
        /// The border color for the rectangle
        /// </summary>
        public string borderColor = "#ffffff";

        /// <summary>
        /// Can be bordered, fill, or line.
        /// Defaults to bordered.
        /// </summary>
        public string mode = "bordered";

        /// <summary>
        /// Creates a rectangle from the given Lua table.
        /// </summary>
        public Rectangle(Table table) : this()
        {
            x = (int)table.Get("x").Number;
            y = (int)table.Get("y").Number;
            width = (int)table.Get("width").Number;
            height = (int)table.Get("height").Number;
            color = table.Get("color").Color();
            borderColor = table.Get("secondaryColor").Color();
            mode = table.Get("mode").String;
        }

        /// <summary>
        /// Draws the rectangle to the current SpriteDestination
        /// </summary>
        public override void Draw()
        {
            if (SpriteDestination.destination == null)
                return;

            SpriteDestination.destination.Add(new JObject()
            {
                {"type", "rectangle"},
                {"x", x - SpriteDestination.offsetX},
                {"y", y - SpriteDestination.offsetY},
                {"width", width},
                {"height", height},
                {"color", borderColor},
                {"thickness", LoveModule.PEN_THICKNESS},
                {"fill", color}
            });
        }

        /// <summary>
        /// Converts the Rectangle to a Lua table.
        /// </summary>
        public override Table ToLuaTable(Script script)
        {
            Table rectangle = base.ToLuaTable(script);

            rectangle["x"] = x;
            rectangle["y"] = y;
            rectangle["width"] = width;
            rectangle["height"] = height;
            rectangle["color"] = script.NewColor(color);
            rectangle["secondaryColor"] = script.NewColor(borderColor);
            rectangle["mode"] = mode;
            rectangle["_type"] = Name;

            rectangle["setColor"] = (Func<DynValue, Table>)((color) =>
            {
                rectangle["color"] = script.NewColor(color.Color());
                return rectangle;
            });

            rectangle["getDrawableSprite"] = () =>
            {
                return (string)rectangle["mode"] == "fill" ? rectangle : new Table(script, DynValue.NewTable(rectangle));
            };

            return rectangle;
        }
    }
}