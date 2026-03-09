using System;
using Edelweiss.RegistryTypes;

namespace Edelweiss.Plugins
{
    /// <summary>
    /// Base class for attributes that add custom functionality to plugin loading
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class PluginLoadAttribute : Attribute
    {
        /// <summary>
        /// Called when the type attached to the attribute is loaded
        /// </summary>
        public virtual void OnLoad(PluginRegistryObject value)
        {
            
        }

        /// <summary>
        /// Called after all types in the plugin are loaded
        /// </summary>
        public virtual void PostLoadTypes(PluginRegistryObject value)
        {
            
        }

        /// <summary>
        /// Called after all plugins are loaded
        /// </summary>
        public virtual void PostLoadPlugins(PluginRegistryObject value)
        {
            
        }
    }
}