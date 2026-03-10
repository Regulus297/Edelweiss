using System;
using Edelweiss.Interop;
using Edelweiss.Plugins;

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

        internal static void SelectTab(CustomTab tab) => TabSelected?.Invoke(tab);
    }
}