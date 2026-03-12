using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Utils
{
    /// <summary>
    /// UI utilities
    /// </summary>
    public static class UI
    {
        /// <summary>
        /// Opens a popup widget taking the data from the given key
        /// </summary>
        /// <param name="key">The resource key for the widget data</param>
        public static void OpenPopupWidget(string key)
        {
            MainVars.OpenWidget(PluginLoader.RequestJson(key));
        }

        /// <summary>
        /// Opens a popup file dialog taking the data from the given key
        /// </summary>
        /// <param name="key">The resource key for the file dialog</param>
        public static void OpenFileDialog(string key)
        {
            MainVars.OpenFileDialog(PluginLoader.RequestJson(key));
        }

        /// <summary>
        /// Opens a popup file dialog using the given JObject as the source
        /// </summary>
        public static void OpenFileDialog(JObject obj)
        {
            MainVars.OpenFileDialog(obj.ToString());
        }

        /// <summary>
        /// Opens a toast-style popup with the given text
        /// </summary>
        /// <param name="text">Either the localization key for the text, or a text literal prefixed with @</param>
        public static void ShowPopup(string text)
        {
            MainVars.ShowPopup(text);
        }
    }
}