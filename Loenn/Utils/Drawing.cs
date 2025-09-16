using System;
using Edelweiss.Utils;
using MoonSharp.Interpreter;

namespace Edelweiss.Loenn.Utils
{
    internal class Drawing : LoennModule
    {
        public override string ModuleName => "utils.drawing";

        public override Table GenerateTable(Script script)
        {
            Table table = new(script);

            table["getCurvePoint"] = (Func<Table, Table, Table, double, DynValue>)((start, stop, control, percent) => GetCurvePoint(script, start, stop, control, percent));

            table["getSimpleCurve"] = (Func<Table, Table, DynValue, DynValue, Table>)((start, stop, ctrl, resolution) =>
            {
                Table control = ctrl.Table ?? new Table(
                    script,
                    DynValue.NewNumber(((double)start[1]+(double)stop[1])/2),
                    DynValue.NewNumber(((double)start[2]+(double)stop[2])/2)
                );
                double res = resolution.IsNil() ? 16 : resolution.Number;
                Table result = new(script);

                for (int i = 0; i < res; i++)
                {
                    (double x, double y) = ((double, double))GetCurvePoint(script, start, stop, control, i / res).Table.Unpack();
                    result.Append(DynValue.NewNumber(x));
                    result.Append(DynValue.NewNumber(y));
                }

                return result;
            });

            table["callKeepOriginalColor"] = (DynValue callback) =>
            {
                string color = LoveModule.color;
                script.Call(callback);
                LoveModule.color = color;
            };

            return table;
        }
        
        private DynValue GetCurvePoint(Script script, Table start, Table stop, Table control, double percent)
        {
            double startMul = (1 - percent) * (1 - percent);
            double controlMul = 2 * (1 - percent) * percent;
            double stopMul = percent * percent;

            double x = (double)start[1] * startMul + (double)control[1] * controlMul + (double)stop[1] * stopMul;
            double y = (double)start[2] * startMul + (double)control[2] * controlMul + (double)stop[2] * stopMul;

            return DynValue.NewTable(script, DynValue.NewNumber(x), DynValue.NewNumber(y));
        }
    }
}