using System;
using Edelweiss.Loenn;
using Edelweiss.Utils;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Drawables
{
    public class Line() : Drawable, ILuaConvertible
    {
        internal int x1, x2, y1, y2;
        internal Table Points
        {
            set
            {
                (x1, y1, x2, y2) = ((int, int, int, int))EdelweissUtils.CastTuple<double, int>(value.Unpack());
            }
        }
        internal string color;
        internal float thickness;
        internal float offsetX, offsetY;
        internal float magnitudeOffset;

        public Line(Table table) : this()
        {
            Points = table.Get("points").Table;
            color = table.Get("color").Color();
            thickness = table.Get<float>("thickness", 1f) * LoveModule.PEN_THICKNESS;
            offsetX = table.Get<float>("offsetX");
            offsetY = table.Get<float>("offsetY");
            magnitudeOffset = table.Get<float>("magnitudeOffset");
        }

        /// <inheritdoc/>
        public override void Draw()
        {
            if (SpriteDestination.destination == null)
                return;

            SpriteDestination.destination.Add(new JObject()
            {
                {"type", "line"},
                {"x1", x1},
                {"x2", x2},
                {"y1", y1},
                {"y2", y2},
                {"color", color},
                {"thickness", thickness}
            });
        }

        /// <summary>
        /// Converts the line to a Lua table
        /// </summary>
        public Table ToLuaTable(Script script)
        {
            Table line = new(script);

            line["_type"] = Name;
            line["points"] = new Table(script, DynValue.NewNumber(x1), DynValue.NewNumber(y1), DynValue.NewNumber(x2), DynValue.NewNumber(y2));
            line["color"] = color;
            line["thickness"] = thickness;
            line["offsetX"] = offsetX;
            line["offsetY"] = offsetY;
            line["magnitudeOffset"] = magnitudeOffset;

            return line;
        }
    }
}