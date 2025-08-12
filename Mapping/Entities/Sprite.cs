using System;
using System.Collections.Generic;
using Edelweiss.Loenn;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities
{
    public class Sprite(string texture, int x, int y, float justificationX, float justificationY, float scaleX, float scaleY, float renderOffsetX, float renderOffsetY, float rotation, int depth) : ILuaConvertible
    {
        internal string texture = texture;
        internal int x = x, y = y;
        internal float justificationX = justificationX, justificationY = justificationY;
        internal float scaleX = scaleX, scaleY = scaleY;
        internal float renderOffsetX = renderOffsetX, renderOffsetY = renderOffsetY;
        internal float rotation = rotation;
        internal int depth = depth;

        public Sprite(string texture) : this(texture, 0, 0, 0.5f, 0.5f, 1, 1, 0, 0, 0, 0) {

        }

        public Sprite(Table table): this(table.Get("texture").String, (int) table.Get("x").Number, (int) table.Get("y").Number,
                                        (float)table.Get("justificationX").Number, (float) table.Get("justificationY").Number,
                                        (float)table.Get("scaleX").Number, (float)table.Get("scaleY").Number, 
                                        (float)table.Get("renderOffsetX").Number, (float)table.Get("renderOffsetY").Number,
                                        (float)table.Get("rotation").Number, (int)table.Get("depth").Number)
        {
            
        }

        public Table ToLuaTable(Script script)
        {
            Table sprite = new(script);

            sprite["texture"] = texture;

            sprite["x"] = x;
            sprite["y"] = y;

            sprite["justificationX"] = justificationX;
            sprite["justificationY"] = justificationY;

            sprite["scaleX"] = scaleX;
            sprite["scaleY"] = scaleY;

            sprite["renderOffsetX"] = renderOffsetX;
            sprite["renderOffsetY"] = renderOffsetY;

            sprite["rotation"] = rotation;

            sprite["depth"] = depth;

            sprite["setPosition"] = (Func<double, double, Table>)((x, y) =>
            {
                // self["x"] = x;
                // self["y"] = y;
                return new Table(script);
            });

            return sprite;
        }

        public JObject ToJObject()
        {
            return new JObject()
            {
                {"type", "pixmap"},
                {"path", "Gameplay/" + texture},
                {"x", 0},
                {"y", 0},
                {"justification", JToken.FromObject(new List<float>() {justificationX, justificationY})},
            };
        }
    }
}