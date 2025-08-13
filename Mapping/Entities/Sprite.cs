using System;
using System.Collections.Generic;
using Edelweiss.Loenn;
using Edelweiss.Plugins;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class Sprite(string texture, int x, int y, float justificationX, float justificationY, float scaleX, float scaleY, float renderOffsetX, float renderOffsetY, float rotation, int depth) : ILuaConvertible
    {
        internal string texture = texture;
        internal int x = x, y = y;
        internal float justificationX = justificationX, justificationY = justificationY;
        internal float scaleX = scaleX, scaleY = scaleY;
        internal float renderOffsetX = renderOffsetX, renderOffsetY = renderOffsetY;
        internal float rotation = rotation;
        internal int depth = depth;

        /// <summary>
        /// Creates a sprite with the given texture key
        /// </summary>
        /// <param name="texture">The texture of the sprite relative to Gameplay/</param>
        public Sprite(string texture) : this(texture, 0, 0, 0.5f, 0.5f, 1, 1, 0, 0, 0, 0)
        {

        }

        /// <summary>
        /// Creates a sprite from the given Lua table
        /// </summary>
        public Sprite(Table table) : this(table.Get("texture").String, (int)table.Get("x").Number, (int)table.Get("y").Number,
                                        (float)table.Get("justificationX").Number, (float)table.Get("justificationY").Number,
                                        (float)table.Get("scaleX").Number, (float)table.Get("scaleY").Number,
                                        (float)table.Get("renderOffsetX").Number, (float)table.Get("renderOffsetY").Number,
                                        (float)table.Get("rotation").Number, (int)table.Get("depth").Number)
        {

        }

        /// <summary>
        /// Converts the sprite to a Lua table that is compatible with Loenn
        /// </summary>
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

            sprite["draw"] = (Action)Draw;

            return sprite;
        }

        internal void Draw() {
            if (SpriteDestination.destination != null)
            {
                SpriteDestination.destination.Add(ToJObject());
            }
        }

        /// <summary>
        /// Converts the sprite to a shape JObject that can be drawn by the frontend
        /// </summary>
        public JObject ToJObject()
        {
            return new JObject()
            {
                {"type", "pixmap"},
                {"path", "Gameplay/" + texture},
                {"x", x - SpriteDestination.offsetX},
                {"y", y - SpriteDestination.offsetY},
                {"justification", JToken.FromObject(new List<float>() {justificationX, justificationY})},
            };
        }
    }

    public class SpriteDestination : IDisposable
    {
        public static JArray destination = null;
        public static int offsetX = 0;
        public static int offsetY = 0;
        public SpriteDestination(JArray shapes, int offsetX, int offsetY)
        {
            destination = shapes;
            SpriteDestination.offsetX = offsetX;
            SpriteDestination.offsetY = offsetY;
        }
        public void Dispose()
        {
            destination = null;
        }
    }
}