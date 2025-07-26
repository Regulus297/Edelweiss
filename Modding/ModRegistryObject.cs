using Edelweiss.RegistryTypes;

namespace Edelweiss.Plugins
{
    public abstract class PluginRegistryObject : IRegistryObject
    {
        public Plugin Plugin { get; internal set; }
        public void OnRegister()
        {
            Load();
        }

        public virtual void Load()
        {
            
        }
    }
}