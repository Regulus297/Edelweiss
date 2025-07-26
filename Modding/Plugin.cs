using Edelweiss.RegistryTypes;

namespace Edelweiss.Plugins
{
    [BaseRegistryObject()]
    public abstract class Plugin : IRegistryObject
    {
        public virtual string ID => GetType().Name;
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