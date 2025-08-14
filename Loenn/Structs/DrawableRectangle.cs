using System;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;
using MoonSharp.Interpreter;

namespace Edelweiss.Loenn.Structs
{
    internal class DrawableRectangle : LoennModule
    {
        public override string ModuleName => "structs.drawable_rectangle";

        public override Table GenerateTable(Script script)
        {
            Table table = new(script);

            table["fromRectangle"] = (Func<CallbackArguments, DynValue>)(cargs =>
            {
                DynValue[] args = cargs.GetArray();
                int x, y, width, height;
                string color, borderColor;
                string mode = args[0].String;
                if (args.MatchesSignature(DataType.String, DataType.Table, DataType.Table))
                {
                    // mode, dimensions, colour
                    x = (int)args[1].Table.Get("x").Number;
                    y = (int)args[1].Table.Get("y").Number;
                    width = (int)args[1].Table.Get("width").Number;
                    height = (int)args[1].Table.Get("height").Number;
                    color = mode == "line" ? "#00000000" : args[2].Color();
                    borderColor = mode == "fill" ? "#00000000" : color;
                }
                else if (args.MatchesSignature(DataType.String, DataType.Table, DataType.Table, DataType.Table))
                {
                    // mode, dimensions, colour, secondary colour
                    x = (int)args[1].Table.Get("x").Number;
                    y = (int)args[1].Table.Get("y").Number;
                    width = (int)args[1].Table.Get("width").Number;
                    height = (int)args[1].Table.Get("height").Number;
                    color = mode == "line" ? "#00000000" : args[2].Color();
                    borderColor = mode == "fill" ? "#00000000" : args[3].Color();
                }
                else if (args.MatchesSignature(DataType.String, DataType.Number, DataType.Number, DataType.Number, DataType.Number, DataType.Table))
                {
                    // mode, x, y, width, height, colour
                    x = (int)args[1].Number;
                    y = (int)args[2].Number;
                    width = (int)args[3].Number;
                    height = (int)args[4].Number;
                    color = mode == "line" ? "#00000000" : args[5].Color();
                    borderColor = mode == "fill" ? "#00000000" : color;
                }
                else if (args.MatchesSignature(DataType.String, DataType.Number, DataType.Number, DataType.Number, DataType.Number, DataType.Table, DataType.Table))
                {
                    // mode, x, y, width, height, colour, secondary color
                    x = (int)args[1].Number;
                    y = (int)args[2].Number;
                    width = (int)args[3].Number;
                    height = (int)args[4].Number;
                    color = mode == "line" ? "#00000000" : args[5].Color();
                    borderColor = mode == "fill" ? "#00000000" : args[6].Color();
                }
                else
                {
                    return DynValue.NewTable(new Rectangle().ToLuaTable(script));
                }
                return DynValue.NewTable(new Rectangle()
                {
                    x = x,
                    y = y,
                    width = width,
                    height = height,
                    color = color,
                    borderColor = borderColor
                }.ToLuaTable(script));
            });

            return table;
        }
    }
}