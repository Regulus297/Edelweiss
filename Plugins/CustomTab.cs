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

        public string LayoutWidget => PluginLoader.RequestJson(LayoutJSON);

        /// <inheritdoc/>
        public override void Load()
        {
            MainVars.Tabs.Add(this);
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