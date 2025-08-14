using System;
using System.Collections.Generic;
using System.Drawing;
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

        internal int sourceX = -1, sourceY = -1, sourceWidth = -1, sourceHeight = -1;

        internal string color = "#ffffff";

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
            sourceX = (int)table.Get("sourceX").Number;
            sourceY = (int)table.Get("sourceY").Number;
            sourceWidth = (int)table.Get("sourceWidth").Number;
            sourceHeight = (int)table.Get("sourceHeight").Number;
            DynValue color = table.Get("color");
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

                this.color = hex;
            }
            else if (color.Type == DataType.String)
                this.color = color.String;
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

            sprite["sourceX"] = sourceX;
            sprite["sourceY"] = sourceY;
            sprite["sourceWidth"] = sourceWidth;
            sprite["sourceHeight"] = sourceHeight;

            sprite["justificationX"] = justificationX;
            sprite["justificationY"] = justificationY;

            sprite["scaleX"] = scaleX;
            sprite["scaleY"] = scaleY;

            sprite["renderOffsetX"] = renderOffsetX;
            sprite["renderOffsetY"] = renderOffsetY;

            sprite["rotation"] = rotation;

            sprite["depth"] = depth;

            sprite["color"] = color;

            sprite["setPosition"] = (Func<double, double, Table>)((x, y) =>
            {
                sprite["x"] = x;
                sprite["y"] = y;
                return sprite;
            });

            sprite["setColor"] = (Func<DynValue, Table>)((color) =>
            {
                sprite["color"] = color;
                return sprite;
            });

            sprite["setScale"] = (Func<double, double, Table>)((x, y) =>
            {
                sprite["scaleX"] = x;
                sprite["scaleY"] = y;
                return sprite;
            });

            sprite["addPosition"] = (Func<double, double, Table>)((x, y) =>
            {
                sprite["x"] = (double)sprite["x"] + x;
                sprite["y"] = (double)sprite["y"] + y;
                return sprite;
            });

            sprite["setJustification"] = (Func<double, double, Table>)((x, y) =>
            {
                sprite["justificationX"] = x;
                sprite["justificationY"] = y;
                return sprite;
            });

            sprite["draw"] = () =>
            {
                new Sprite(sprite).Draw();
            };

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
                {"sourceX", sourceX},
                {"sourceY", sourceY},
                {"sourceWidth", sourceWidth},
                {"sourceHeight", sourceHeight},
                {"rotation", rotation * 180/MathF.PI},
                {"scaleX", scaleX},
                {"scaleY", scaleY},
                {"color", color}
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