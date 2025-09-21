using System;
using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Loenn;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Plugins;
using Edelweiss.Utils;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Drawables
{
    /// <summary>
    /// 
    /// </summary>
    public class Sprite : Drawable, ILuaConvertible
    {
        internal string texture;
        internal int x, y;
        internal float justificationX, justificationY;
        internal float scaleX, scaleY;
        internal float renderOffsetX, renderOffsetY;
        internal float rotation;
        internal int depth;

        internal int sourceX = -1, sourceY = -1, sourceWidth = -1, sourceHeight = -1;
        internal int atlasX, atlasY, atlasWidth, atlasHeight;

        internal string color = "#ffffff";

        /// <summary>
        /// 
        /// </summary>
        internal Sprite(string texture, int x, int y, float justificationX, float justificationY, float scaleX, float scaleY, float renderOffsetX, float renderOffsetY, float rotation, int depth)
        {
            this.texture = texture;
            this.x = x;
            this.y = y;
            this.justificationX = justificationX;
            this.justificationY = justificationY;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.renderOffsetX = renderOffsetX;
            this.renderOffsetY = renderOffsetY;
            this.rotation = rotation;
            this.depth = depth;

            atlasX = -1;
            atlasY = -1;
            atlasWidth = -1;
            atlasHeight = -1;

            TextureData textureData;
            if ((textureData = CelesteModLoader.GetTextureData("Gameplay/" + texture)) == null)
                return;

            atlasX = textureData.atlasX;
            atlasY = textureData.atlasY;
            atlasWidth = textureData.atlasWidth;
            atlasHeight = textureData.atlasHeight;
        }

        /// <summary>
        /// Creates a sprite with the given texture key
        /// </summary>
        /// <param name="texture">The texture of the sprite relative to Gameplay/</param>
        public Sprite(string texture) : this(texture, 0, 0, 0.5f, 0.5f, 1, 1, 0, 0, 0, 0)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public Sprite() : this("")
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
            color = table.Get("color").Color();
            if (table.Get("offsetX").IsNotNil())
            {
                justificationX = 0;
                x += (int)table.Get("offsetX").Number;
            }
            if (table.Get("offsetY").IsNotNil())
            {
                justificationY = 0;
                y += (int)table.Get("offsetY").Number;
            }
        }

        /// <summary>
        /// Converts the sprite to a Lua table that is compatible with Loenn
        /// </summary>
        public override Table ToLuaTable(Script script)
        {
            Table sprite = base.ToLuaTable(script);

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

            sprite["_type"] = Name;

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

            sprite["setAlpha"] = (Func<double, Table>)((alpha) =>
            {
                int a = (int)(alpha * 255);
                Table color = script.NewColor(sprite.Get("color").Color()).Table;
                color[4] = DynValue.NewNumber(a);
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

            sprite["useRelativeQuad"] = (Func<double, double, double, double, Table>)((x, y, width, height) =>
            {
                sprite["sourceX"] = x;
                sprite["sourceY"] = y;
                sprite["offsetX"] = 0;
                sprite["offsetY"] = 0;
                sprite["sourceWidth"] = width;
                sprite["sourceHeight"] = height;
                return sprite;
            });

            sprite["setOffset"] = (double x, double y) =>
            {
                sprite["offsetX"] = x;
                sprite["offsetY"] = y;
                return sprite;
            };

            Table mt = new(script);
            mt["__index"] = (Func<Table, string, DynValue>)((t, key) =>
            {
                if (key == "meta")
                {
                    return DynValue.NewTable(CelesteModLoader.GetTextureData("Gameplay/" + t["texture"].ToString()).ToLuaTable(script));
                }
                return DynValue.Nil;
            });

            sprite.MetaTable = mt;

            return sprite;
        }

        /// <inheritdoc/>
        public override void Draw()
        {
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
                {"x", x - SpriteDestination.offsetX + (int)renderOffsetX},
                {"y", y - SpriteDestination.offsetY + (int)renderOffsetY},
                {"justification", JToken.FromObject(new List<float>() {justificationX, justificationY})},
                {"sourceX", sourceX < 0 ? atlasX : sourceX + atlasX},
                {"sourceY", sourceY < 0 ? atlasY : sourceY + atlasY},
                {"sourceWidth", sourceWidth < 0 ? atlasWidth : sourceWidth},
                {"sourceHeight", sourceHeight < 0 ? atlasHeight : sourceHeight},
                {"rotation", rotation * 180/MathF.PI},
                {"scaleX", scaleX},
                {"scaleY", scaleY},
                {"color", color}
            };
        }
    }

    /// <summary>
    /// A disposable object to draw sprites to a particular JArray.
    /// </summary>
    public class SpriteDestination : IDisposable
    {
        /// <summary>
        /// The array to which sprites are drawn
        /// </summary>
        public static JArray destination = null;
        /// <summary>
        /// The horizontal offset for the sprites
        /// </summary>
        public static int offsetX = 0;
        /// <summary>
        /// The vertical offset for the sprites
        /// </summary>
        public static int offsetY = 0;

        int myOffsetX, myOffsetY;

        /// <summary>
        /// Creates a SpriteDestination for the given JArray and offsets
        /// </summary>
        /// <param name="shapes">The array to which sprites are drawn</param>
        /// <param name="offsetX">The horizontal offset for the sprites</param>
        /// <param name="offsetY">The vertical offset for the sprites</param>
        public SpriteDestination(JArray shapes, int offsetX, int offsetY)
        {
            destination = shapes;
            SpriteDestination.offsetX += offsetX;
            SpriteDestination.offsetY += offsetY;
            myOffsetX = offsetX;
            myOffsetY = offsetY;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            destination = null;
            offsetX -= myOffsetX;
            offsetY -= myOffsetY;
        }
    }
}