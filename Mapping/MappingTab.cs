using System;
using System.Collections.Generic;
using System.Linq;
using Edelweiss.Mapping.Tools;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping
{
    internal class MappingTab : CustomTab
    {
        public override string LayoutJSON => "Edelweiss:mapping_tab";

        public override string ToolbarJSON => "Edelweiss:mapping_toolbar";

        public override string DisplayName => "Mapping";

        internal static SyncedVariable tools = new("Edelweiss:Tools");
        internal static SyncedVariable modes = new("Edelweiss:Modes");
        internal static SyncedVariable layers = new("Edelweiss:Layers");
        internal static SyncedVariable materials = new("Edelweiss:Materials");

        public override void PostSetupContent()
        {
            tools.Value = Registry.registry[typeof(MappingTool)].values.Select(t => ((MappingTool)t).DisplayName).ToList();
            modes.Value = new List<string>();
            layers.Value = new List<string>();
            materials.Value = new List<string>();
            base.PostSetupContent();
        }

        public override void HandleToolbarClick(string actionName, JObject extraData)
        {
            switch (actionName)
            {
                case "Create Room":
                    Console.WriteLine("User wants to create a room");

                    NetworkManager.SendPacket(Netcode.OPEN_POPUP_FORM, FormLoader.LoadForm("Edelweiss:Forms/room_creation").ToString());
                    break;
                case "File/New File":
                    Console.WriteLine("User wants to create a new file");
                    NetworkManager.SendPacket(Netcode.MODIFY_ITEM_SHAPE, new JObject()
                    {
                        {"widget", "Mapping/MainView"},
                        {"item", "a-01"},
                        {"index", 1},
                        {"data", new JObject() {
                            {"tileData", "This changed!"}
                        }}
                    });
                    break;
                case "File/Open Recent/Blahaj":
                    Console.WriteLine("User wants a blahaj");
                    break;
            }
        }
    }
}