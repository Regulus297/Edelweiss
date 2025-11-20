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
        internal static long MapFileNetcode { get; private set; }
        internal static MappingTool selectedTool;

        internal static MapData map = null;

        internal static string filePath = "";

        public override void Load()
        {
            MouseMovedNetcode = Plugin.CreateNetcode("MouseMoved", false);
            RoomMouseNetcode = Plugin.CreateNetcode("RoomMouse", false);
            MaterialFavouritedNetcode = Plugin.CreateNetcode("MaterialFavourited", false);
            MaterialSearchedNetcode = Plugin.CreateNetcode("MaterialSearched", false);
            MapFileNetcode = Plugin.CreateNetcode("MapFile", false);
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
                case "fileMenu/openMap":
                    NetworkManager.SendPacket(Netcode.OPEN_FILE_DIALOG, new JObject()
                    {
                        {"file", true},
                        {"path", MainPlugin.CelesteDirectory},
                        {"mode", "load"},
                        {"pattern", "*.bin"},
                        {"submit", new JObject() {
                            {"netcode", MapFileNetcode},
                            {"extraData", new JObject() {
                                {"type", "load"}
                            }}
                        }}
                    });
                    break;

                case "fileMenu/saveMapAs":
                    NetworkManager.SendPacket(Netcode.OPEN_FILE_DIALOG, new JObject()
                    {
                        {"file", true},
                        {"path", MainPlugin.CelesteDirectory},
                        {"mode", "save"},
                        {"pattern", "*.bin"},
                        {"submit", new JObject() {
                            {"netcode", MapFileNetcode},
                            {"extraData", new JObject() {
                                {"type", "save"}
                            }}
                        }}
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
                case "mapMenu/mapMeta":
                    NetworkManager.SendPacket(Netcode.OPEN_POPUP_FORM, FormLoader.LoadForm("Edelweiss:Forms/map_meta", map.meta.ToJObject()).ToString());
                    break;
            }
        }

        internal static void RedrawComplete()
        {
            JObject room = PluginLoader.RequestJObject("Edelweiss:GraphicsItems/room");
            foreach (RoomData r in map.rooms)
            {
                room["x"] = r.x;
                room["y"] = r.y;
                room["shapes"][2]["color"] = RoomData.GetColor(r.color);
                room["shapes"][2]["width"] = r.width;
                room["shapes"][2]["height"] = r.height;
                room["shapes"][0]["tileData"] = r.bgTileData.TileData;
                room["shapes"][0]["width"] = r.width / 8;
                room["shapes"][0]["height"] = r.height / 8;
                room["shapes"][1]["tileData"] = r.fgTileData.TileData;
                room["shapes"][1]["width"] = r.width / 8;
                room["shapes"][1]["height"] = r.height / 8;
                room["name"] = r.name;

                NetworkManager.SendPacket(Netcode.ADD_ITEM, new JObject()
                {
                    {"widget", "Mapping/MainView"},
                    {"item", room}
                });

                foreach (Entity entity in r.entities.Values)
                {
                    entity.Draw();
                }
            }
        }

        /// <summary>
        /// Gets the entity object with the given name coming from the frontend
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Entity GetEntity(string name)
        {
            var split = name.Split(':');
            string roomName = split[0];
            RoomData room = map.rooms.FirstOrDefault(r => r.name == roomName);
            return room?.entities.GetValueOrDefault(split[1]);
        }
    }
}