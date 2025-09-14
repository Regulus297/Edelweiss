using System;
using Edelweiss.Utils;
using MoonSharp.Interpreter;

namespace Edelweiss.Loenn.Utils
{
    internal class UtilsModule : LoennModule
    {
        public override string ModuleName => "utils";
        public override bool Global => true;

        public override Table GenerateTable(Script script)
        {
            Table utils = new(script);

            utils["titleCase"] = (Func<string, string>)EdelweissUtils.CamelCaseToText;

            utils["distanceSquared"] = (Func<double, double, double, double, double>)((x1, y1, x2, y2) => ((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));

            utils["log"] = (Action<DynValue>)MainPlugin.Instance.Logger.Log;

            utils["getColor"] = (Func<DynValue, DynValue>)(v =>
            {
                string color = v.Color();
                return script.NewColor(color);
            });

            utils["rectangle"] = (Func<double, double, double, double, Table>)((x, y, w, h) =>
            {
                Table rect = new Table(script);

                rect["x"] = w < 0 ? w + x : x;
                rect["y"] = h < 0 ? h + y : y;
                rect["width"] = Math.Abs(w);
                rect["height"] = Math.Abs(h);
                return rect;
            });

            return utils;
        }
    }
}