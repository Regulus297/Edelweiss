using Edelweiss.RegistryTypes;

namespace Edelweiss.Plugins
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PluginRegistryObject : IRegistryObject
    {
        /// <summary>
        /// The plugin that this object is defined by
        /// </summary>
        public Plugin Plugin { get; internal set; }

        /// <summary>
        /// The name of the object
        /// </summary>
        public virtual string Name => GetType().Name;

        /// <summary>
        /// The full identifier for the object, consisting of its defining plugin's ID and the object's name
        /// </summary>
        public string FullName => $"{Plugin.ID}:{Name}";

        /// <summary>
        /// 
        /// </summary>
        public void OnRegister()
        {
            Load();
        }

        /// <summary>
        /// Called when the object is entered into the registry
        /// </summary>
        public virtual void Load()
        {

        }

        /// <summary>
        /// Called after all plugins have been loaded.
        /// </summary>
        public virtual void PostSetupContent()
        {

        }
    }
}