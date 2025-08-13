using System;
using Edelweiss.Utils;
using MoonSharp.Interpreter;

namespace Edelweiss.Loenn.Utils
{
    internal class UtilsModule : LoennModule
    {
        public override string ModuleName => "utils";

        public override Table GenerateTable(Script script)
        {
            Table utils = new(script);

            utils["titleCase"] = (Func<string, string>)EdelweissUtils.CamelCaseToText;

            utils["distanceSquared"] = (Func<double, double, double, double, double>)((x1, y1, x2, y2) => ((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));

            utils["log"] = (Action<DynValue>)MainPlugin.Instance.Logger.Log;

            return utils;
        }
    }
}