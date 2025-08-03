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
        internal static SyncedVariable toolIDs = new("Edelweiss:ToolIDs");
        internal static SyncedVariable modes = new("Edelweiss:Modes");
        internal static SyncedVariable layers = new("Edelweiss:Layers");
        internal static SyncedVariable materials = new("Edelweiss:Materials");
        internal static SyncedVariable materialIDs = new("Edelweiss:MaterialIDs");
        internal static SyncedVariable selectedMode = new("Edelweiss:SelectedMode", 0);
        internal static SyncedVariable selectedLayer = new("Edelweiss:SelectedLayer", 0);
        internal static SyncedVariable selectedMaterial = new("Edelweiss:SelectedMaterial", 0);

        internal static long MouseMovedNetcode { get; private set; }
        internal static long RoomMouseNetcode { get; private set; }
        internal static long MaterialFavouritedNetcode { get; private set; }

        internal static MappingTool selectedTool;

        public override void Load()
        {
            MouseMovedNetcode = Plugin.CreateNetcode("MouseMoved", false);
            RoomMouseNetcode = Plugin.CreateNetcode("RoomMouse", false);
            MaterialFavouritedNetcode = Plugin.CreateNetcode("MaterialFavourited", false);
        }

        public override void PostSetupContent()
        {
            tools.Value = Registry.registry[typeof(MappingTool)].values.Select(t => ((MappingTool)t).DisplayName).ToList();
            toolIDs.Value = Registry.registry[typeof(MappingTool)].values.Select(t => ((MappingTool)t).FullName).ToList();
            modes.Value = new List<string>();
            layers.Value = new List<string>();
            materials.Value = new List<string>();
            materialIDs.Value = new List<string>();
            base.PostSetupContent();
            NetworkManager.SendPacket(Netcode.ADD_ITEM, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", PluginLoader.RequestJObject("Edelweiss:GraphicsItems/cursor_ghost")}
            });
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
                    break;
                case "File/Open Recent/Blahaj":
                    Console.WriteLine("User wants a blahaj");
                    break;
            }
        }
    }
}