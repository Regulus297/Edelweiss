using System;
using Edelweiss.RegistryTypes;

namespace Edelweiss.Plugins
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class PluginLoadAttribute : Attribute
    {
        public virtual void OnLoad(PluginRegistryObject value)
        {
            
        }

        public virtual void PostLoadTypes(PluginRegistryObject value)
        {
            
        }

        public virtual void PostLoadPlugins(PluginRegistryObject value)
        {
            
        }
    }
}