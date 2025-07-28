using System;
using Edelweiss.RegistryTypes;

namespace Edelweiss.Plugins
{
    [BaseRegistryObject()]
    public abstract class Plugin : IRegistryObject
    {
        public virtual string ID => GetType().Name;
        internal event Action OnPostSetupContent;
        public void OnRegister()
        {
            Load();
        }

        public virtual void Load()
        {

        }

        internal void PostLoad()
        {
            PostSetupContent();
            OnPostSetupContent.Invoke();
        }

        public virtual void PostSetupContent()
        {
            
        }
    }
}