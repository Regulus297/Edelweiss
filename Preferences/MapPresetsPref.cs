using Edelweiss.ModManagement;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Preferences
{
    internal class MapPresetsPref: PluginSaveablePreference
    {
        internal static MapPresetsPref Instance => Registry.registry[typeof(PluginSaveablePreference)].GetValue<MapPresetsPref>();
        internal static JObject CustomMapPresets => (JObject)Instance.Value;
        public override void SetDefaultValue()
        {
            Value = new JObject();
        }
    }
}