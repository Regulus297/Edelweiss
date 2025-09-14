using System;
using System.Linq;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;
using MoonSharp.Interpreter;

namespace Edelweiss.Loenn.Structs
{
    internal class DrawableLine : LoennModule
    {
        public override string ModuleName => "structs.drawable_line";

        public override Table GenerateTable(Script script)
        {
            Table table = new(script);

            table["fromPoints"] = (Func<Table, DynValue, DynValue, DynValue, DynValue, DynValue>)((points, color, offsetX, offsetY, magnitudeOffset) =>
            {
                Line line = new()
                {
                    Points = points,
                    color = color.Color(),
                    offsetX = offsetX.Type == DataType.Number ? (float)offsetX.Number : 0,
                    offsetY = offsetY.Type == DataType.Number ? (float)offsetY.Number : 0,
                    magnitudeOffset = magnitudeOffset.Type == DataType.Number ? (float)magnitudeOffset.Number : 0
                };
                return DynValue.NewTable(line.ToLuaTable(script));
            });

            return table;
        }
    }
}