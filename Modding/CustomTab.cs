using System.Collections.Generic;
using Edelweiss.Network;
using Edelweiss.RegistryTypes;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Plugins
{
    [BaseRegistryObject()]
    public abstract class CustomTab : PluginRegistryObject
    {
        public abstract string LayoutJSON { get; }
        public abstract string ToolbarJSON { get; }
        public abstract string DisplayName { get; }
        public virtual int LoadWeight => 0;

        public static Dictionary<string, CustomTab> registeredTabs = [];


        public override void PostSetupContent()
        {
            NetworkManager.SendPacket(Netcode.REGISTER_JSON_SCENE, new JObject()
            {
                {"name", DisplayName},
                {"internalName", FullName},
                {"json", PluginLoader.RequestJson(LayoutJSON)}
            });
            registeredTabs[FullName] = this;
        }


        internal void Select()
        {
            // When the tab is selected, tell the UI to update the toolbar
            NetworkManager.SendPacket(Netcode.REGISTER_TOOLBAR, PluginLoader.RequestJson(ToolbarJSON));

            OnSelect();
        }

        public virtual void OnSelect()
        {

        }

        public virtual void OnDeselect()
        {

        }

        public virtual void HandleToolbarClick(string actionName)
        {
            
        }
    }
}