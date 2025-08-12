using System;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;
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
                Language.LanguageVar.Value = value;
            }
        }

        public override void SetDefaultValue()
        {
            Value = "en_GB";
        }
    }
}