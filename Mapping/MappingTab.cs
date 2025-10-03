using System;
using System.Collections.Generic;
using System.Linq;
using Edelweiss.Mapping.Entities;
using Edelweiss.Mapping.SaveLoad;
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

        internal static SyncedVariable tools = new("Edelweiss:Tools", null, "Mapping/ToolList");
        internal static SyncedVariable toolIDs = new("Edelweiss:ToolIDs", null);
        internal static SyncedVariable modes = new("Edelweiss:Modes", null, "Mapping/ModeList");
        internal static SyncedVariable layers = new("Edelweiss:Layers", null, "Mapping/LayerList");
        internal static SyncedVariable materials = new("Edelweiss:Materials", null, "Mapping/MaterialList");
        internal static SyncedVariable materialIDs = new("Edelweiss:MaterialIDs", null);
        internal static SyncedVariable selectedMode = new("Edelweiss:SelectedMode", 0);
        internal static SyncedVariable selectedLayer = new("Edelweiss:SelectedLayer", 0);
        internal static SyncedVariable selectedMaterial = new("Edelweiss:SelectedMaterial", 0);
        internal static SyncedVariable rooms = new("Edelweiss:Rooms", null, "Mapping/RoomList");

        internal static string searchTerm = "";

        internal static long MouseMovedNetcode { get; private set; }
        internal static long RoomMouseNetcode { get; private set; }
        internal static long MaterialFavouritedNetcode { get; private set; }
        internal static long MaterialSearchedNetcode { get; private set; }
        internal static long SaveMapNetcode { get; private set; }
        internal static MappingTool selectedTool;

        internal static MapData map = null;

        internal static string filePath = "";

        public override void Load()
        {
            MouseMovedNetcode = Plugin.CreateNetcode("MouseMoved", false);
            RoomMouseNetcode = Plugin.CreateNetcode("RoomMouse", false);
            MaterialFavouritedNetcode = Plugin.CreateNetcode("MaterialFavourited", false);
            MaterialSearchedNetcode = Plugin.CreateNetcode("MaterialSearched", false);
            SaveMapNetcode = Plugin.CreateNetcode("SaveMap", false);
        }

        public override void PostSetupContent()
        {
            tools.Value = Registry.registry[typeof(MappingTool)].values.Select(t => ((MappingTool)t).DisplayName).ToList();
            toolIDs.Value = Registry.registry[typeof(MappingTool)].values.Select(t => ((MappingTool)t).FullName).ToList();
            modes.Value = new List<string>();
            layers.Value = new List<string>();
            materials.Value = new List<string>();
            materialIDs.Value = new List<string>();
            rooms.Value = new List<string>();
            map = new();
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
                case "createRoom":
                    NetworkManager.SendPacket(Netcode.OPEN_POPUP_FORM, FormLoader.LoadForm("Edelweiss:Forms/room_creation").ToString());
                    break;
                case "fileMenu/saveMapAs":
                    NetworkManager.SendPacket(Netcode.OPEN_FILE_DIALOG, new JObject()
                    {
                        {"file", true},
                        {"path", MainPlugin.CelesteDirectory},
                        {"mode", "save"},
                        {"pattern", "*.bin"},
                        {"submit", SaveMapNetcode}
                    });
                    break;
                case "fileMenu/saveMap":
                    if (string.IsNullOrEmpty(filePath))
                    {
                        goto case "fileMenu/saveMapAs";
                    }
                    try
                    {
                        MapSaveLoad.SaveMap(map, filePath);
                    }
                    catch (Exception e)
                    {
                        MainPlugin.Instance.Logger.Error($"Error saving map: {e}");
                    }
                    break;
            }
        }
    }
}