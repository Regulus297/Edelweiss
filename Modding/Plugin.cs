using System;
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
    }
}