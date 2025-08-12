using Edelweiss.Network;
using Edelweiss.Plugins;
using Edelweiss.Preferences;
using Edelweiss.RegistryTypes;

namespace Edelweiss.Utils
{
    public static class Language
    {
        public static string CurrentLanguage => Registry.registry[typeof(PluginSaveablePreference)].GetValue<LanguagePref>().Value.ToString();
        internal static SyncedVariable LanguageVar => new SyncedVariable("Language");
        public static string GetText(string key)
        {
            var localization = PluginLoader.localization[CurrentLanguage];
            if (localization.TryGetValue(key, out string text))
            {
                return text;
            }
            return key;
        }

        public static string GetTextOrDefault(string key, string defaultValue = null)
        {
            return TryGetText(key, out string text) ? text: defaultValue;
        }

        public static bool TryGetText(string key, out string text)
        {
            var localization = PluginLoader.localization[CurrentLanguage];
            return localization.TryGetValue(key, out text);
        }
    }
}