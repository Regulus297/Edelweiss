using Edelweiss.Interop;
using Edelweiss.Plugins;
using Edelweiss.Preferences;
using Edelweiss.RegistryTypes;

namespace Edelweiss
{
    public class MainInterop : PluginInterop
    {
        public void SetCelesteDirectory(string directory)
        {
            Registry.registry[typeof(PluginSaveablePreference)].GetValue<CelesteDirectoryPref>().StringValue = directory;
        }
    }
}