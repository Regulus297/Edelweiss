using Edelweiss.Network;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Utils
{
    /// <summary>
    /// Utilities for interacting with the UI from the backend
    /// </summary>
    public static class UI
    {
        /// <summary>
        /// Shows a popup on the UI with the given text and lasting for the specified duration
        /// </summary>
        public static void ShowPopup(string text, int duration = 2000)
        {
            NetworkManager.SendPacket(Netcode.SHOW_POPUP, new JObject()
            {
                {"text", text},
                {"duration", duration}
            });
        }

        /// <summary>
        /// Shows a popup on the UI with the text associated with the given localization key and lasting for the specified duration
        /// </summary>
        public static void ShowLocalizedPopup(string textKey, int duration = 2000) => ShowPopup(Language.GetText(textKey), duration);

        /// <summary>
        /// Shows a popup form taking the JSON from the given resource key
        /// </summary>
        /// <param name="formKey">The resource key to the JSON file containing the form source</param>
        /// <param name="defaults">The default values to initialize the form to, if any</param>
        public static void OpenForm(string formKey, JObject defaults = null)
        {
            NetworkManager.SendPacket(Netcode.OPEN_POPUP_FORM, FormLoader.LoadForm(formKey, defaults).ToString());
        }
    }
}