using System;
using System.Collections.Generic;
using Edelweiss.Network;
using Edelweiss.RegistryTypes;

namespace Edelweiss.Plugins
{
    /// <summary>
    /// The basic plugin class
    /// </summary>
    [BaseRegistryObject()]
    public abstract class Plugin : IRegistryObject
    {
        /// <summary>
        /// The ID of the plugin. Every plugin needs to have a unique identifier.
        /// </summary>
        public virtual string ID => GetType().Name;
        internal event Action OnPostSetupContent;

        /// <summary>
        /// 
        /// </summary>
        public void OnRegister()
        {
            Load();
        }

        /// <summary>
        /// Called when the plugin is loaded
        /// </summary>
        public virtual void Load()
        {

        }

        internal void PostLoad()
        {
            PostSetupContent();
            OnPostSetupContent.Invoke();
        }

        /// <summary>
        /// Called after all plugins have been loaded
        /// </summary>
        public virtual void PostSetupContent()
        {

        }

        /// <summary>
        /// Creates a netcode with a given identifier prefixed by the plugin's ID.
        /// </summary>
        /// <param name="name">The name of the netcode</param>
        /// <param name="positive">If true, the generated netcode will be positive.</param>
        public long CreateNetcode(string name, bool positive) => Netcode.CreateNetcode($"{ID}:{name}", positive);

        /// <summary>
        /// Creates an environment variable with a given name
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <param name="getValue">The function to get the value of the variable</param>
        public void CreateVar(string name, Func<object> getValue) => PluginVars.AddVar($"{ID}:{name}", getValue);
    }
}