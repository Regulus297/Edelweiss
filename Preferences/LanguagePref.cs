using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Preferences
{
    internal class LanguagePref : PluginSaveablePreference
    {
        public override void SetDefaultValue()
        {
            Value = "en_gb";
        }
    }
}