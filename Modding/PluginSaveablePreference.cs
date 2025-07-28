using System;
using System.IO;
using Edelweiss.RegistryTypes;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Plugins
{
    [BaseRegistryObject()]
    public abstract class PluginSaveablePreference : PluginRegistryObject
    {
        public JToken Value { get; set; }
        public static JToken AllPrefs { get; private set; }


        public sealed override void Load()
        {
            Value = AllPrefs.Value<JToken>(FullName);
            if (Value == null)
            {
                SetDefaultValue();
            }
        }

        public virtual void SetDefaultValue()
        {
            
        }

        
        internal static void LoadPrefs()
        {
            string prefPath = InstantiatePrefsFile();

            using (StreamReader reader = new(prefPath))
            {
                AllPrefs = JToken.Parse(reader.ReadToEnd());
            }
        }

        internal static void SavePrefs()
        {
            string prefPath = InstantiatePrefsFile();
            using (StreamWriter writer = new(prefPath))
            {
                JObject obj = JObject.Parse("{}");
                Registry.ForAll<PluginSaveablePreference>(pref =>
                {
                    obj.Add(pref.FullName, pref.Value);
                });
                writer.WriteLine(obj.ToString());
            }
        }

        private static string InstantiatePrefsFile()
        {
            string prefPath = Path.Join(Directory.GetCurrentDirectory(), "prefs.json");
            if (!File.Exists(prefPath))
            {
                File.Open(prefPath, FileMode.CreateNew).Close();
                using (StreamWriter writer = new(prefPath))
                {
                    writer.WriteLine("{}");
                }
            }
            return prefPath;
        }
    }
}