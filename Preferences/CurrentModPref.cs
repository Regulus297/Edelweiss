using Edelweiss.Modding;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Preferences
{
    internal class CurrentModPref : PluginSaveablePreference
    {
        public override JToken Value
        {
            get => base.Value;
            set {
                base.Value = value;
                ModdingTab.CurrentMod.Value = ModData.Load(value.ToString());
            }
        }
        public override void PrepForSave()
        {
            base.Value = ModdingTab.CurrentMod.Value?.ModDirectory;
        }
    }
}