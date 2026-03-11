using System;
using Edelweiss.Interop;
using Edelweiss.Plugins;
using Edelweiss.Utils;

namespace Edelweiss
{
    /// <summary>
    /// Class containing the main variables
    /// </summary>
    public class MainVars : Loadable, ISyncable
    {
        string ISyncable.Name() => MainPlugin.Instance.ID;

        /// <summary>
        /// The list of all defined tabs
        /// </summary>
        public static readonly BindableList<CustomTab> Tabs = [];

        /// <summary>
        /// The dictionary of all python plugin keys to their paths
        /// </summary>
        public static readonly BindableDictionary<string, string> PythonPlugins = [];

        /// <summary>
        /// The currently selected language
        /// </summary>
        public static readonly BindableVariable<string> CurrentLanguage = "en_gb";

        /// <summary>
        /// Invoked when a tab is selected
        /// </summary>
        public static event Action<CustomTab> TabSelected;

        /// <summary>
        /// Invoked with the JSON source of the file when <see cref="UI.OpenPopupWidget(string)"/> is called.
        /// </summary>
        public static event Action<string> OnOpenWidget;
        
        /// <summary>
        /// Invoked with the JSON source of the file when <see cref="UI.OpenFileDialog(string)"/> is called.
        /// </summary>
        public static event Action<string> OnOpenFileDialog;
        

        internal static void SelectTab(CustomTab tab) => TabSelected?.Invoke(tab);
        internal static void OpenWidget(string data) => OnOpenWidget?.Invoke(data);
        internal static void OpenFileDialog(string data) => OnOpenFileDialog?.Invoke(data);
    }
}