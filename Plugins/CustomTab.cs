using System.Collections.Generic;
using Edelweiss.MVC;
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
        [ModelProperty] public abstract string DisplayName { get; }
        [ModelProperty] public string InternalName => FullName;

        /// <summary>
        /// The list of all custom tabs indexed by their internal IDs
        /// </summary>
        public static readonly Dictionary<string, CustomTab> registeredTabs = [];

        public sealed override void Load()
        {
            PluginLoader.PostLoadUI += () => MainPlugin.TabData.RegisterTab(this);
            PostLoad();
        }

        public virtual void PostLoad()
        {
            
        }

        internal void Select()
        {
            // When the tab is selected, tell the UI to update the toolbar
            OnSelect();
        }
        
        /// <summary>
        /// Called when the user switches to this tab in the editor
        /// </summary>
        public virtual void OnSelect()
        {

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