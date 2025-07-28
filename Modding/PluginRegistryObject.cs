using Edelweiss.RegistryTypes;

namespace Edelweiss.Plugins
{
    public abstract class PluginRegistryObject : IRegistryObject
    {
        public Plugin Plugin { get; internal set; }

        public virtual string Name => GetType().Name;
        public string FullName => $"{Plugin.ID}:{Name}";
        public void OnRegister()
        {
            Load();
        }

        public virtual void Load()
        {

        }

        public virtual void PostSetupContent()
        {
            
        }
    }
}