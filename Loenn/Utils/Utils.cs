using MoonSharp.Interpreter;

namespace Edelweiss.Loenn.Utils
{
    internal class UtilsModule : LoennModule
    {
        public override string ModuleName => "utils";

        public override Table GenerateTable(Script script)
        {
            Table utils = new(script);

            return utils;
        }
    }
}