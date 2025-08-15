using System;
using Edelweiss.Loenn;
using Edelweiss.Utils;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Drawables
{
    public class Rectangle() : Drawable, ILuaConvertible
    {
        public int x = 0, y = 0;
        public int width = 0, height = 0;
        public string color = "#ffffff";
        public string borderColor = "#ffffff";
        public string mode = "bordered";

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

        public Table ToLuaTable(Script script)
        {
            Table rectangle = new(script);

            rectangle["x"] = x;
            rectangle["y"] = y;
            rectangle["width"] = width;
            rectangle["height"] = height;
            rectangle["color"] = color;
            rectangle["secondaryColor"] = borderColor;
            rectangle["mode"] = mode;
            rectangle["_type"] = Name;

            rectangle["setColor"] = (Func<DynValue, Table>)((color) =>
            {
                rectangle["color"] = color;
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