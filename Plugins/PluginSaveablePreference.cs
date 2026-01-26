using System;
using System.IO;
using Edelweiss.RegistryTypes;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Plugins
{
    /// <summary>
    ///
    /// </summary>
    [BaseRegistryObject()]
    public abstract class PluginSaveablePreference : PluginRegistryObject
    {
        /// <summary>
        /// The value of the preference
        /// </summary>
        public virtual JToken Value { get; set; }

        /// <summary>
        /// The loaded JSON object containing all currently loaded preferences
        /// </summary>
        public static JToken AllPrefs { get; private set; }

        /// <inheritdoc/>
        public sealed override void Load()
        {
            Value = AllPrefs.Value<JToken>(FullName);
            if (Value == null)
            {
                PluginLoader.PostLoadTypes += SetDefaultValue;
            }
        }
        
        /// <summary>
        /// Called if the preference was not found in the saved preferences.
        /// </summary>
        public virtual void SetDefaultValue()
        {

        }

        /// <summary>
        /// Called before the preference is saved, to perform tasks necessary to save correctly
        /// </summary>
        public virtual void PrepForSave()
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
                    pref.PrepForSave();
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