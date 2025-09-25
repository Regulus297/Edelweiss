using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Edelweiss.Loenn;
using Edelweiss.Plugins;
using Edelweiss.Utils;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Drawables
{
    /// <summary>
    /// A class that represents a drawable line
    /// </summary>
    public class Line() : Drawable
    {
        internal List<float> points = [];
        internal Table Points
        {
            set
            {
                points = value.Values.Select(v => (float)v.Number).ToList();
            }
        }
        internal string color;
        internal float thickness;
        internal float offsetX, offsetY;
        internal float magnitudeOffset;

        /// <summary>
        /// Creates a line from a given Lua table
        /// </summary>
        public Line(Table table) : this()
        {
            Points = table.Get("points").Table;
            color = table.Get("color").Color();
            thickness = (float)table.Get<double>("thickness", 1f) * LoveModule.PEN_THICKNESS;
            offsetX = (float)table.Get<double>("offsetX");
            offsetY = (float)table.Get<double>("offsetY");
            magnitudeOffset = (float)table.Get<double>("magnitudeOffset");
        }

        /// <summary>
        /// Creates a line between the given two points
        /// </summary>
        public Line(Point a, Point b, string color = "#ffffffff", float thickness = 1f * LoveModule.PEN_THICKNESS) : this()
        {
            points = [a.X, a.Y, b.X, b.Y];
            this.color = color;
            this.thickness = thickness;
        }

        /// <summary>
        /// Creates a line between the given coordinates
        /// </summary>
        public Line(float x1, float y1, float x2, float y2, string color = "#ffffffff", float thickness = 1f * LoveModule.PEN_THICKNESS) : this()
        {
            points = [x1, y1, x2, y2];
            this.color = color;
            this.thickness = thickness;
        }

        /// <inheritdoc/>
        public override void Draw()
        {
            if (SpriteDestination.destination == null)
                return;

            for (int i = 0; i + 3 < points.Count; i += 2)
            {
                SpriteDestination.destination.Add(new JObject()
                {
                    {"type", "line"},
                    {"x1", points[0 + i] - SpriteDestination.offsetX + offsetX},
                    {"y1", points[1 + i] - SpriteDestination.offsetY + offsetY},
                    {"x2", points[2 + i] - SpriteDestination.offsetX + offsetX},
                    {"y2", points[3 + i] - SpriteDestination.offsetY + offsetY},
                    {"color", color},
                    {"thickness", thickness},
                    {"depth", depth}
                });
            }
        }

        /// <summary>
        /// Converts the line to a Lua table
        /// </summary>
        public override Table ToLuaTable(Script script)
        {
            Table line = base.ToLuaTable(script);

            line["_type"] = Name;
            line["points"] = new Table(script, points.Select(t => DynValue.NewNumber(t)).ToArray());
            line["color"] = script.NewColor(color);
            line["thickness"] = thickness;
            line["offsetX"] = offsetX;
            line["offsetY"] = offsetY;
            line["magnitudeOffset"] = magnitudeOffset;

            line["getDrawableSprite"] = () =>
            {
                return DynValue.NewTable(script, DynValue.NewTable(line));
            };

            line["setOffset"] = (double x, double y) =>
            {
                line["offsetX"] = x;
                line["offsetY"] = y;
                return line;
            };

            line["setColor"] = (DynValue color) =>
            {
                line["color"] = color;
                return line;
            };

            return line;
        }
    }
}