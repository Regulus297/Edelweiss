using Edelweiss.Network;
using Edelweiss.Plugins;
using Edelweiss.Preferences;
using Edelweiss.RegistryTypes;

namespace Edelweiss.Utils
{
    /// <summary>
    /// Handles all localization-related code
    /// </summary>
    public static class Language
    {
        /// <summary>
        /// The currently selected language key.
        /// </summary>
        public static string CurrentLanguage => Registry.registry[typeof(PluginSaveablePreference)].GetValue<LanguagePref>().Value.ToString();
        internal static SyncedVariable LanguageVar => new SyncedVariable("Language");

        /// <summary>
        /// Returns the localization for a given key
        /// </summary>
        /// <returns>The localization if it exists, the key itself if it doesn't</returns>
        public static string GetText(string key)
        {
            var localization = PluginLoader.localization[CurrentLanguage];
            if (localization.TryGetValue(key, out string text))
            {
                return text;
            }
            return key;
        }

        /// <summary>
        /// Returns the localization for a given key and the default value if it doesn't exist.
        /// </summary>
        public static string GetTextOrDefault(string key, string defaultValue = null)
        {
            return TryGetText(key, out string text) ? text : defaultValue;
        }

        /// <summary>
        /// Attempts to get the localization for a given key.
        /// </summary>
        /// <returns>Whether or not the localization exists.</returns>
        public static bool TryGetText(string key, out string text)
        {
            var localization = PluginLoader.localization[CurrentLanguage];
            return localization.TryGetValue(key, out text);
        }
    }
}