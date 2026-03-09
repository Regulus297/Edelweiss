using System.Collections.Generic;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;

namespace Edelweiss.Interop
{
    /// <summary>
    /// Base class for an interop class that defines methods that can be called by the UI
    /// </summary>
    [BaseRegistryObject]
    public abstract class PluginInterop : PluginRegistryObject
    {
        /// <summary>
        /// The dictionary of all interop keys to their instances
        /// </summary>
        public static readonly Dictionary<string, PluginInterop> Interops = [];

        /// <inheritdoc/>
        public sealed override void Load()
        {
            Interops[FullName] = this;
            OnLoad();
        }

        /// <summary>
        /// Called when the Interop is loaded
        /// </summary>
        public virtual void OnLoad()
        {
            
        }
    }
}