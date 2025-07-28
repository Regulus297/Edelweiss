using System;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Tabs
{
    internal class MappingTab : CustomTab
    {
        public override string LayoutJSON => "Edelweiss:mapping_tab";

        public override string ToolbarJSON => "Edelweiss:mapping_toolbar";

        public override string DisplayName => "Mapping";

        public override void HandleToolbarClick(string actionName)
        {
            switch (actionName)
            {
                case "Create Room":
                    Console.WriteLine("User wants to create a room");
                    NetworkManager.SendPacket(Netcode.ADD_ITEM, new JObject()
                    {
                        {"widget", "Mapping/MainView"},
                        {"item", PluginLoader.RequestJObject("Edelweiss:GraphicsItems/room")}
                    });
                    break;
                case "File/New File":
                    Console.WriteLine("User wants to create a new file");
                    break;
                case "File/Open Recent/Blahaj":
                    Console.WriteLine("User wants a blahaj");
                    break;
            }
        }
    }
}