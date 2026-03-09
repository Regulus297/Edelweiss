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
                MainVars.CurrentLanguage.Value = value.ToString();
            }
        }

        public override void SetDefaultValue()
        {
            MainVars.CurrentLanguage.Value = "en_gb";
        }

        public override void PrepForSave()
        {
            base.Value = MainVars.CurrentLanguage.Value;
        }
    }
}