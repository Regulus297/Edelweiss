using System;
using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
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

                return new NinePatch(ninePatch).ToLuaTable(script);
            });

            return table;
        }
    }
}