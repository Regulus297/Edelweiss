using System;
using System.Collections.Generic;
using Edelweiss.Loenn;
using Edelweiss.Plugins;
using Edelweiss.Utils;
using MoonSharp.Interpreter;

namespace Edelweiss.Mapping.Drawables
{
    /// <summary>
    /// A drawable object that draws tileable textures
    /// </summary>
    public class NinePatch() : Drawable, ILuaConvertible
    {
        /// <summary>
        /// The texture key of the NinePatch
        /// </summary>
        public string texture = "";

        /// <summary>
        /// 
        /// </summary>
        public string mode = "";
        /// <summary>
        /// 
        /// </summary>
        public string borderMode = "", fillMode = "";

        /// <summary>
        /// 
        /// </summary>
        public int x = 0, y = 0;

        /// <summary>
        /// 
        /// </summary>
        public int width = 0, height = 0;

        /// <summary>
        /// 
        /// </summary>
        public int tileSize = 0, tileWidth = 0, tileHeight = 0;

        /// <summary>
        /// 
        /// </summary>
        public int borderLeft = 0, borderRight = 0, borderTop = 0, borderBottom = 0;

        /// <summary>
        /// 
        /// </summary>
        public string color;

        /// <summary>
        /// Creates a NinePatch from the given Lua table
        /// </summary>
        public NinePatch(Table table) : this()
        {
            texture = table.Get("texture").String;
            mode = table.Get("mode").String;
            borderMode = table.Get("borderMode").String;
            fillMode = table.Get("fillMode").String;
            x = (int)table.Get("drawX").Number;
            y = (int)table.Get("drawY").Number;
            width = (int)table.Get("drawWidth").Number;
            height = (int)table.Get("drawHeight").Number;
            tileSize = (int)table.Get("tileSize").Number;
            tileWidth = (int)table.Get("tileWidth").Number;
            tileHeight = (int)table.Get("tileHeight").Number;
            borderLeft = (int)table.Get("borderLeft").Number;
            borderRight = (int)table.Get("borderRight").Number;
            borderTop = (int)table.Get("borderTop").Number;
            borderBottom = (int)table.Get("borderBottom").Number;
            color = table.Get("color").Color();
        }

        /// <summary>
        /// Draws the NinePatch to the current SpriteDestination.
        /// </summary>
        public override void Draw()
        {
            foreach (Sprite sprite in GetSprites())
            {
                sprite.x += x;
                sprite.y += y;
                sprite.Draw();
            }
        }

        internal List<Sprite> GetSprites()
        {

            TextureData data = CelesteModLoader.GetTextureData("Gameplay/" + texture);
            List<Sprite> sprites = [];

            // Corners
            if (width > 0 && height > 0 && borderLeft > 0 && borderTop > 0)
            {
                sprites.Add(new(texture)
                {
                    sourceX = 0,
                    sourceY = 0,
                    sourceWidth = borderLeft,
                    sourceHeight = borderTop,
                    justificationX = 0,
                    justificationY = 0
                });
            }
            if (width > 0 && height > 0 && borderRight > 0 && borderTop > 0)
            {
                sprites.Add(new(texture)
                {
                    x = (int)width - borderRight,
                    sourceX = data.width-borderRight,
                    sourceY = 0,
                    sourceWidth = borderRight,
                    sourceHeight = borderTop,
                    justificationX = 0,
                    justificationY = 0
                });
            }
            if (width > 0 && height > 0 && borderLeft > 0 && borderBottom > 0)
            {
                sprites.Add(new(texture)
                {
                    y = (int)height - borderBottom,
                    sourceX = 0,
                    sourceY = data.height-borderBottom,
                    sourceWidth = borderLeft,
                    sourceHeight = borderBottom,
                    justificationX = 0,
                    justificationY = 0
                });
            }
            if (width > 0 && height > 0 && borderRight > 0 && borderBottom > 0)
            {
                sprites.Add(new(texture)
                {
                    x = (int)width - borderRight,
                    y = (int)height - borderBottom,
                    sourceX = data.width-borderRight,
                    sourceY = data.height-borderBottom,
                    sourceWidth = borderLeft,
                    sourceHeight = borderBottom,
                    justificationX = 0,
                    justificationY = 0
                });
            }

            // Horizontal edges
            for (int x = borderLeft; x < width - borderRight; x += tileWidth)
            {
                int sourceX = (x - borderLeft) % (data.width - (borderLeft + borderRight));
                sourceX += borderLeft;
                if (borderTop > 0)
                {
                    sprites.Add(new Sprite(texture)
                    {
                        x = x,
                        y = 0,
                        sourceX = sourceX,
                        sourceY = 0,
                        sourceWidth = tileWidth,
                        sourceHeight = borderTop,
                        justificationX = 0,
                        justificationY = 0
                    });
                }
                if (borderBottom > 0)
                {
                    sprites.Add(new Sprite(texture)
                    {
                        x = x,
                        y = (int)height - borderBottom,
                        sourceX = sourceX,
                        sourceY = data.height-borderBottom,
                        sourceWidth = tileWidth,
                        sourceHeight = borderBottom,
                        justificationX = 0,
                        justificationY = 0
                    });
                }
            }

            // Vertical edges
            for (int y = borderTop; y < height - borderTop; y += tileHeight)
            {
                int sourceY = (y - borderTop) % (data.width - (borderTop + borderBottom));
                sourceY += borderTop;
                if (borderLeft > 0)
                {
                    sprites.Add(new Sprite(texture)
                    {
                        x = 0,
                        y = y,
                        sourceX = 0,
                        sourceY = sourceY,
                        sourceWidth = borderLeft,
                        sourceHeight = tileHeight,
                        justificationX = 0,
                        justificationY = 0
                    });
                }
                if (borderRight > 0)
                {
                    sprites.Add(new Sprite(texture)
                    {
                        x = width - borderRight,
                        y = y,
                        sourceX = data.width-borderRight,
                        sourceY = sourceY,
                        sourceWidth = borderRight,
                        sourceHeight = tileHeight,
                        justificationX = 0,
                        justificationY = 0
                    });
                }
            }

            // Fill
            for (int x = borderLeft; x < width - borderRight; x += tileWidth)
            {
                for (int y = borderTop; y < height - borderTop; y += tileHeight)
                {
                    int sourceY = (y - borderTop) % (data.width - (borderTop + borderBottom));
                    sourceY += borderTop;
                    int sourceX = (x - borderLeft) % (data.width - (borderLeft + borderRight));
                    sourceX += borderLeft;
                    sprites.Add(new Sprite(texture)
                    {
                        x = x,
                        y = y,
                        sourceX = sourceX,
                        sourceY = sourceY,
                        sourceWidth = tileWidth,
                        sourceHeight = tileHeight,
                        justificationX = 0,
                        justificationY = 0
                    });
                }
            }

            foreach (Sprite sprite in sprites)
            {
                sprite.color = color;
            }

            return sprites;
        }

        /// <summary>
        /// Converts the NinePatch to a Lua table.
        /// </summary>
        public override Table ToLuaTable(Script script)
        {
            Table ninePatch = base.ToLuaTable(script);

            ninePatch["_type"] = Name;
            ninePatch["texture"] = texture;
            ninePatch["mode"] = mode;
            ninePatch["borderMode"] = borderMode;
            ninePatch["fillMode"] = fillMode;
            ninePatch["drawX"] = x;
            ninePatch["drawY"] = y;
            ninePatch["drawWidth"] = width;
            ninePatch["drawHeight"] = height;
            ninePatch["tileSize"] = tileSize;
            ninePatch["tileWidth"] = tileWidth;
            ninePatch["tileHeight"] = tileHeight;
            ninePatch["borderLeft"] = borderLeft;
            ninePatch["borderRight"] = borderRight;
            ninePatch["borderTop"] = borderTop;
            ninePatch["borderBottom"] = borderBottom;
            ninePatch["color"] = script.NewColor(DynValue.NewString(color).Color());

            ninePatch["getDrawableSprite"] = () =>
            {
                Table spriteTable = new(script);
                foreach (Sprite sprite in GetSprites())
                {
                    sprite.x += x;
                    sprite.y += y;
                    sprite.color = ninePatch.Get("color").Color();
                    spriteTable.Append(DynValue.NewTable(sprite.ToLuaTable(script)));
                }
                return spriteTable;
            };

            ninePatch["setColor"] = (DynValue color) =>
            {
                ninePatch["color"] = color;
                return ninePatch;
            };

            return ninePatch;
        }
    }
}