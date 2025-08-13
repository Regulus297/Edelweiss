using System;
using System.Collections.Generic;
using Edelweiss.Mapping.Entities;
using Edelweiss.Utils;
using MoonSharp.Interpreter;

namespace Edelweiss.Loenn.Structs
{
    internal class DrawableNinePatch : LoennModule
    {
        public override string ModuleName => "structs.drawable_nine_patch";

        public override Table GenerateTable(Script script)
        {
            Table table = new(script);

            table["fromTexture"] = (Func<string, Table, double, double, double, double, Table>)((texture, options, x, y, width, height) =>
            {
                Table ninePatch = new(script);

                ninePatch["texture"] = texture;
                ninePatch["mode"] = options.Get("mode").CastToString() ?? "fill";
                ninePatch["borderMode"] = options.Get("borderMode").CastToString() ?? "repeat";
                ninePatch["fillMode"] = options.Get("fillMode").CastToString() ?? "repeat";
                ninePatch["drawX"] = x;
                ninePatch["drawY"] = y;
                ninePatch["drawWidth"] = width;
                ninePatch["drawHeight"] = height;
                ninePatch["tileSize"] = options.Get("tileSize").CastToNumber() ?? 8;
                ninePatch["tileWidth"] = options.Get("tileWidth").CastToNumber() ?? ninePatch["tileSize"];
                ninePatch["tileHeight"] = options.Get("tileHeight").CastToNumber() ?? ninePatch["tileSize"];
                ninePatch["borderLeft"] = options.Get("borderLeft").CastToNumber() ?? options.Get("border").CastToNumber() ?? ninePatch["tileWidth"];
                ninePatch["borderRight"] = options.Get("borderRight").CastToNumber() ?? options.Get("border").CastToNumber() ?? ninePatch["tileWidth"];
                ninePatch["borderTop"] = options.Get("borderTop").CastToNumber() ?? options.Get("border").CastToNumber() ?? ninePatch["tileHeight"];
                ninePatch["borderBottom"] = options.Get("borderBottom").CastToNumber() ?? options.Get("border").CastToNumber() ?? ninePatch["tileHeight"];

                ninePatch["getDrawableSprite"] = () =>
                {
                    List<Sprite> sprites = new();
                    int borderLeft = (int)(double)ninePatch["borderLeft"];
                    int borderRight = (int)(double)ninePatch["borderRight"];
                    int borderTop = (int)(double)ninePatch["borderTop"];
                    int borderBottom = (int)(double)ninePatch["borderBottom"];

                    int tileWidth = (int)(double)ninePatch["tileWidth"];
                    int tileHeight = (int)(double)ninePatch["tileHeight"];

                    TextureData data = CelesteModLoader.GetTextureData("Gameplay/" + texture);


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
                            sourceX = -borderRight,
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
                            sourceY = -borderBottom,
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
                            sourceX = -borderRight,
                            sourceY = -borderBottom,
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
                        sprites.Add(new Sprite(texture)
                        {
                            x = x,
                            y = (int)height - borderBottom,
                            sourceX = sourceX,
                            sourceY = -borderBottom,
                            sourceWidth = tileWidth,
                            sourceHeight = borderBottom,
                            justificationX = 0,
                            justificationY = 0
                        });
                    }

                    // Vertical edges
                    for (int y = borderTop; y < height - borderTop; y += tileHeight)
                    {
                        int sourceY = (y - borderTop) % (data.width - (borderTop + borderBottom));
                        sourceY += borderTop;
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
                        sprites.Add(new Sprite(texture)
                        {
                            x = (int)width - borderRight,
                            y = y,
                            sourceX = -borderRight,
                            sourceY = sourceY,
                            sourceWidth = borderRight,
                            sourceHeight = tileHeight,
                            justificationX = 0,
                            justificationY = 0
                        });
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

                    Table spriteTable = new(script);
                    foreach (Sprite sprite in sprites)
                    {
                        sprite.x += SpriteDestination.offsetX;
                        sprite.y += SpriteDestination.offsetY;
                        spriteTable.Append(DynValue.NewTable(sprite.ToLuaTable(script)));
                    }

                    return spriteTable;
                };

                return ninePatch;
            });

            return table;
        }
    }
}