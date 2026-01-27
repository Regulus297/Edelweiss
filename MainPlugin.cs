using Edelweiss.MVC;
using Edelweiss.Plugins;
using Edelweiss.Preferences;
using Edelweiss.RegistryTypes;

namespace Edelweiss
{
    internal sealed class MainPlugin : Plugin
    {
        internal static MainPlugin Instance { get; private set; }
        public override string ID => "Edelweiss";
        public static readonly TabModelData TabData = new TabModelData();

        internal static string CelesteDirectory => Registry.registry[typeof(PluginSaveablePreference)].GetValue<CelesteDirectoryPref>().Value.ToString();

        public override void Load()
        {
            Instance = this;
            Model.Create(TabData).Sync("Edelweiss:TabModelData");
        }
    }
}