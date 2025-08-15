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
        internal static Dictionary<string, Func<Script, Table>> globalModules = [];

        internal static DynValue RequireModule(Script script, string module)
        {
            if (createdModules.TryGetValue(module, out var table))
                return DynValue.NewTable(table(script));

            throw new ScriptRuntimeException($"Unrecognized module {module}");
        }

        /// <inheritdoc/>
        public sealed override void Load()
        {
            if (Global)
            {
                globalModules[TableName == "" ? ModuleName : TableName] = GenerateTable;
            }
            createdModules[ModuleName] = GenerateTable;
        }

        /// <summary>
        /// The name of the Loenn module this replaces.
        /// </summary>
        public abstract string ModuleName { get; }

        /// <summary>
        /// The name of the table this module defines. If the module is global and this is overridden, 
        /// this will be the name of the global instead of the module name.
        /// </summary>
        public virtual string TableName => "";

        /// <summary>
        /// If false, the module is used using require(). If true, the module is accessible as a global
        /// </summary>
        public virtual bool Global => false;

        /// <summary>
        /// Generates the table for the module
        /// </summary>
        public abstract Table GenerateTable(Script script);
    }
}