using System;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;
using MoonSharp.Interpreter;

namespace Edelweiss.Loenn.Structs
{
    internal class DrawableRectangle : LoennModule
    {
        public override string ModuleName => "structs.drawable_rectangle";

        public override bool Global => true;
        public override string TableName => "drawableRectangle";

        public override Table GenerateTable(Script script)
        {
            Table table = new(script);

            table["fromRectangle"] = (Func<string, DynValue, DynValue, DynValue, DynValue, DynValue, DynValue, DynValue>)((mode, ix, iy, w, h, col, col2) =>
            {
                int x, y, width, height;
                string color, borderColor;
                if (ix.Type == DataType.Table)
                {
                    // mode, dimensions, colour, [secondary colour]
                    x = (int)ix.Table.Get("x").Number;
                    y = (int)ix.Table.Get("y").Number;
                    width = (int)ix.Table.Get("width").Number;
                    height = (int)ix.Table.Get("height").Number;
                    color = mode == "line" ? "#00000000" : iy.Color();
                    borderColor = mode == "fill" ? "#00000000" : w.IsNil() ? color : w.Color();
                }
                else
                {
                    // mode, x, y, width, height, colour, [secondary colour]
                    x = (int)ix.Number;
                    y = (int)iy.Number;
                    width = (int)w.Number;
                    height = (int)h.Number;
                    color = mode == "line" ? "#00000000" : col.Color();
                    borderColor = mode == "fill" ? "#00000000" : col2.IsNil() ? color : col2.Color();
                }
                return DynValue.NewTable(new Rectangle()
                {
                    x = x,
                    y = y,
                    width = width,
                    height = height,
                    color = color,
                    borderColor = borderColor,
                    mode = mode
                }.ToLuaTable(script));
            });

            return table;
        }
    }
}