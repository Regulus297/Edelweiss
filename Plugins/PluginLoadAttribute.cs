using System;
using Edelweiss.RegistryTypes;

namespace Edelweiss.Plugins
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
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

        public virtual void PostLoadUI(IRegistryObject value)
        {
            
        }
    }
}