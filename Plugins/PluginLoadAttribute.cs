using System;
using Edelweiss.RegistryTypes;

namespace Edelweiss.Plugins
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class PluginLoadAttribute : Attribute
    {
        public virtual void OnLoad(IRegistryObject value)
        {
            
        }

        public virtual void PostLoadTypes(IRegistryObject value)
        {
            
        }

        public virtual void PostLoadPlugins(IRegistryObject value)
        {
            
        }
    }
}