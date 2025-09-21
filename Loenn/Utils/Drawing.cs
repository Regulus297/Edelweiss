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

            table["getDashedLineSegments"] = (double x1, double y1, double x2, double y2, DynValue d, DynValue s) =>
            {
                double dash = d.IsNil() ? 6 : d.Number;
                double space = d.IsNil() ? 4 : s.Number;

                double length = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
                double progress = 0;
                Table segments = new(script);

                while (progress < length)
                {
                    double startPercent = progress / length;
                    double endPercent = Math.Min(progress + dash, length) / length;
                    double startX = x1 + (x2 - x1) * startPercent;
                    double startY = y1 + (y2 - y1) * startPercent;
                    double endX = x1 + (x2 - x1) * endPercent;
                    double endY = y1 + (y2 - y1) * endPercent;

                    segments.Append(DynValue.NewTable(script, DynValue.NewNumber(startX), DynValue.NewNumber(startY), DynValue.NewNumber(endX), DynValue.NewNumber(endY)));

                    progress += dash + space;
                }
                return segments;
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