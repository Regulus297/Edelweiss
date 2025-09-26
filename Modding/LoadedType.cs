using Edelweiss.RegistryTypes;

namespace Edelweiss.Plugins
{
    /// <summary>
    /// Base class for a type that needs to be loaded by the mod loader but does not fall under any other category
    /// </summary>
    [BaseRegistryObject()]
    public abstract class LoadedType : PluginRegistryObject
    {
        
    }
}