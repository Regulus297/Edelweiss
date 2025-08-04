using MoonSharp.Interpreter;

namespace Edelweiss.Loenn
{
    public interface ILuaConvertible
    {
        public Table ToLuaTable(Script script);
    }
}