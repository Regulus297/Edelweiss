using Edelweiss.Plugins;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Preferences
{
    internal class LanguagePref : PluginSaveablePreference
    {

        public override JToken Value
        {
            set
            {
                base.Value = value;
            }
        }

        public override void SetDefaultValue()
        {
            Value = "en_gb";
        }
    }
}