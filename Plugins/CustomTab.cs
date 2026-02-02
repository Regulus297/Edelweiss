using System.Collections.Generic;
using Edelweiss.RegistryTypes;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Plugins
{
    /// <summary>
    /// Registers a custom UI tab, like the mapping tab.
    /// </summary>
    [BaseRegistryObject]
    public abstract class CustomTab : PluginRegistryObject
    {
        /// <summary>
        /// The key for the JSON file containing the layout of the tab
        /// </summary>
        public abstract string LayoutJSON { get; }

        /// <summary>
        /// The key for the JSON file containing the toolbar for the tab
        /// </summary>
        public abstract string ToolbarJSON { get; }

        /// <summary>
        /// The name that will be displayed in the editor
        /// </summary>
        public abstract string DisplayName { get; }

        /// <summary>
        /// The list of all custom tabs indexed by their internal IDs
        /// </summary>
        public static Dictionary<string, CustomTab> registeredTabs = [];

        /// <inheritdoc/>
        public override void Load()
        {
            MainPlugin.Tabs.Value.Add(this);
            MainPlugin.Tabs.ForceUpdate();
        }


        internal void Select()
        {
            OnSelect();
        }
        
        /// <summary>
        /// Called when the user switches to this tab in the editor
        /// </summary>
        public virtual void OnSelect()
        {
            Plugin.Logger.Debug($"Tab selected: {FullName}");
        }

        /// <summary>
        /// Called when the user switches away from this tab in the editor
        /// </summary>
        public virtual void OnDeselect()
        {

        }

        /// <summary>
        /// Called when a toolbar action belonging to this tab's toolbar is triggered
        /// </summary>
        /// <param name="actionName">The identifier for the action that was triggered</param>
        /// <param name="extraData">Any additional data that was requested in the toolbar's JSON file</param>
        public virtual void HandleToolbarClick(string actionName, JObject extraData)
        {

        }
    }
}