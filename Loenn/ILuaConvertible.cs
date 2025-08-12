using MoonSharp.Interpreter;

namespace Edelweiss.Loenn
{
    /// <summary>
    /// Makes a class convertible into a Loenn-compatible Lua table.
    /// </summary>
    public interface ILuaConvertible
    {
        /// <summary>
        /// Converts the class into a Lua table.
        /// </summary>
        public Table ToLuaTable(Script script);
    }
}