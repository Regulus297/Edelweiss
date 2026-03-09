using Edelweiss.Interop;
using Edelweiss.Plugins;
using Edelweiss.Preferences;
using Edelweiss.RegistryTypes;
using Edelweiss.Utils;

namespace Edelweiss
{
    /// <summary>
    /// Interop for general purpose functions
    /// </summary>
    public class MainInterop : PluginInterop
    {
        private CustomTab current = null;
        /// <summary>
        /// Sets the Celeste Directory to the given value
        /// </summary>
        public void SetCelesteDirectory(string directory)
        {
            Registry.registry[typeof(PluginSaveablePreference)].GetValue<CelesteDirectoryPref>().StringValue = directory;
        }

        /// <summary>
        /// Changes the currently selected tab
        /// </summary>
        public void ChangeTab(CustomTab tab)
        {
            current?.OnDeselect();
            current = tab;
            current.Select();
        }

        /// <summary>
        /// Logs a message through the main logger.
        /// </summary>
        public void Debug(object message)
        {
            MainPlugin.Instance.Logger.Debug(message);
        }

        /// <summary>
        /// Gets the localization for a given key
        /// </summary>
        public string GetLocalization(string key, string fallback = null) => Language.GetTextOrDefault(key, fallback ?? key);
    }
}