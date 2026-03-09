using Edelweiss.RegistryTypes;

namespace Edelweiss.Plugins
{
    /// <summary>
    /// Base class for all objects that need to be loaded by the PluginLoader but do not have any other function.
    /// </summary>
    [BaseRegistryObject]
    public abstract class Loadable: PluginRegistryObject
    {
        
    }
}