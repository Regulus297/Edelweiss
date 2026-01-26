using System.Collections.Generic;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;

namespace Edelweiss.Interop
{
    [BaseRegistryObject]
    public abstract class PluginInterop : PluginRegistryObject
    {
        public static readonly Dictionary<string, PluginInterop> Interops = [];
        public sealed override void Load()
        {
            Interops[FullName] = this;
            OnLoad();
        }

        public virtual void OnLoad()
        {
            
        }
    }
}