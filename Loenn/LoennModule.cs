using System;
using System.Collections.Generic;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;
using MoonSharp.Interpreter;

namespace Edelweiss.Loenn
{
    [BaseRegistryObject()]
    public abstract class LoennModule : PluginRegistryObject
    {
        internal static Dictionary<string, Func<Script, Table>> createdModules = [];

        internal static DynValue RequireModule(Script script, string module)
        {
            if (createdModules.TryGetValue(module, out var table))
                return DynValue.NewTable(table(script));

            throw new ScriptRuntimeException($"Unrecognized module {module}");
        }

        public sealed override void Load()
        {
            createdModules[ModuleName] = GenerateTable;
        }
        public abstract string ModuleName { get; }
        public abstract Table GenerateTable(Script script);
    }
}