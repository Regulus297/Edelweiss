using System.Collections.Generic;
using Edelweiss.Interop;
using Edelweiss.Plugins;
using Edelweiss.Preferences;
using Edelweiss.RegistryTypes;

namespace Edelweiss
{
    internal sealed class MainPlugin : Plugin
    {
        internal static MainPlugin Instance { get; private set; }
        public override string ID => "Edelweiss";

        internal static string CelesteDirectory => Registry.registry[typeof(PluginSaveablePreference)].GetValue<CelesteDirectoryPref>().StringValue;

        public override void Load()
        {
            Instance = this;
        }
    }
}