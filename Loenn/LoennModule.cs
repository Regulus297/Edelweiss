using System;
using System.Collections.Generic;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;
using MoonSharp.Interpreter;

namespace Edelweiss.Loenn
{
    /// <summary>
    /// Base class for a custom implementation of a Loenn module.
    /// </summary>
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

        /// <inheritdoc/>
        public sealed override void Load()
        {
            createdModules[ModuleName] = GenerateTable;
        }

        /// <summary>
        /// The name of the Loenn module this replaces.
        /// </summary>
        public abstract string ModuleName { get; }

        /// <summary>
        /// Generates the table for the module
        /// </summary>
        public abstract Table GenerateTable(Script script);
    }
}