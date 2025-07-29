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

        public override void HandleToolbarClick(string actionName, JObject extraData)
        {
            switch (actionName)
            {
                case "Create Room":
                    Console.WriteLine("User wants to create a room");
                    NetworkManager.SendPacket(Netcode.OPEN_POPUP_FORM, PluginLoader.RequestJson("Edelweiss:Forms/room_creation_form"));
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