using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Edelweiss.Mapping.Entities;
using Edelweiss.Mapping.Keybinds;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Tools
{
    [LoadAfter(typeof(RectTool))]
    internal class EntityTool : MappingTool
    {
        Dictionary<string, string> cachedMaterials = null;
        List<string> favourites = [];
        internal string previousSearchTerm = "";
        internal bool recache = false;
        private string lastSelectedMaterial = "";

        private int startTileX, startTileY;
        Entity ghostEntity;
        bool dragging = false;

        bool redrawGhost = false;

        public override void Load()
        {
            CelesteModLoader.PostLoadMods += () =>
            {
                recache = true;
            };
            PluginKeybind.AddListener<RotateKeybind>(RotateGhost);
            PluginKeybind.AddListener<VerticalFlipKeybind>(() => FlipGhost(false));
            PluginKeybind.AddListener<HorizontalFlipKeybind>(() => FlipGhost(true));
            PluginKeybind.AddListener<CycleKeybind>(CycleGhost);
        }

        private void RotateGhost()
        {
            if (this != MappingTab.selectedTool)
            {
                return;
            }

            var rotate = ghostEntity?.Rotate(1);
            if (rotate == true)
            {
                RefreshGhost();
            }
        }

        private void FlipGhost(bool horizontal) {
            if (this != MappingTab.selectedTool)
            {
                return;
            }

            var flip = ghostEntity?.Flip(horizontal, !horizontal);
            if (flip == true)
            {
                RefreshGhost();
            }
        }

        private void CycleGhost()
        {
            if (this != MappingTab.selectedTool)
            {
                return;
            }

            var cycle = ghostEntity?.Cycle(1);
            if (cycle == true)
            {
                RefreshGhost();
            }
        }

        public override Dictionary<string, string> GetMaterials()
        {
            if (cachedMaterials == null || previousSearchTerm != MappingTab.searchTerm || recache)
            {
                previousSearchTerm = MappingTab.searchTerm;
                recache = false;
                cachedMaterials = [];
                foreach (string material in favourites)
                {
                    if (!CelesteModLoader.entities.ContainsKey(material))
                        continue;
                    if (IsSearched(CelesteModLoader.entities[material].DisplayName))
                        cachedMaterials[material] = "★ " + CelesteModLoader.entities[material].DisplayName;
                }
                foreach (string mod in CelesteModLoader.entityMods)
                {
                    foreach (string entity in CelesteModLoader.modEntities[mod])
                    {
                        if (favourites.Contains(entity))
                            continue;

                        EntityData entityData = CelesteModLoader.entities[entity];
                        if (IsSearched(entityData.DisplayName))
                            cachedMaterials[entity] = entityData.DisplayName;
                    }
                }
            }
            return cachedMaterials;
        }

        public override void OnFavourited(string material)
        {
            favourites.Toggle(material);
            recache = true;
        }

        public override JToken SaveFavourites()
        {
            return new JObject() {
                {"entities", new JArray(favourites.ToArray())}
            };
        }

        public override void LoadFavourites(JObject data)
        {
            JArray entities = data.Value<JArray>("entities");
            if (entities != null)
            {
                favourites = entities.Select(t => t.ToString()).ToList();
            }
        }

        public override void MouseClick(JObject room, float x, float y)
        {
            (startTileX, startTileY) = EdelweissUtils.ToTileCoordinate(x, y);
        }

        public override void MouseDrag(JObject room, float x, float y)
        {
            dragging = true;
            (int tileX, int tileY) = EdelweissUtils.ToTileCoordinate(x, y);
            List<bool> canResize = ghostEntity.entityData.CanResize(RoomData.Default, ghostEntity);
            bool changed = false;
            if (canResize[0] || canResize[1])
            {
                changed = true;
                if (canResize[0])
                {
                    if (tileX <= startTileX)
                    {
                        NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
                        {
                            {"widget", "Mapping/MainView"},
                            {"item", "cursorGhost"},
                            {"data", new JObject() {
                                {"x", tileX * 8}
                            }}
                        });
                    }
                    ghostEntity.Resize((Math.Abs(tileX - startTileX) + 1) * 8, ghostEntity.height);
                }
                if (canResize[1])
                {
                    if (tileY <= startTileY)
                    {
                        NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
                        {
                            {"widget", "Mapping/MainView"},
                            {"item", "cursorGhost"},
                            {"data", new JObject() {
                                {"y", tileY * 8}
                            }}
                        });
                    }
                    ghostEntity.Resize(ghostEntity.width, (Math.Abs(tileY - startTileY) + 1) * 8);
                    
                }
            }
            else if (ghostEntity.nodes.Count == 1)
            {
                changed = true;
                ghostEntity.nodes[0] = new Point(tileX * 8 - startTileX * 8, tileY * 8 - startTileY * 8);
            }
            if (changed)
            {
                RefreshGhost();
            }
        }

        private void RefreshGhost()
        {
            NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", "cursorGhost"},
                {"action", "clear"}
            });
            ghostEntity.Draw(0.5f, "cursorGhost", 1);
        }

        public override void MouseRelease(JObject room, float x, float y)
        {
            dragging = false;
            RoomData backendRoom = MappingTab.map.rooms.FirstOrDefault(r => r.name == room.Value<string>("name"));
            (int tileX, int tileY) = EdelweissUtils.ToTileCoordinate(x, y);
            Entity created = Entity.DefaultFromData(ghostEntity.entityData, backendRoom);
            created.data = ghostEntity.data;
            created._name = ghostEntity._name;
            created._id = backendRoom.entities.Count().ToString();
            created.x = 8 * startTileX;
            created.y = 8 * startTileY;

            List<bool> canResize = ghostEntity.entityData.CanResize(RoomData.Default, ghostEntity);
            if (canResize[0] || canResize[1])
            {
                if (canResize[0] && tileX < startTileX)
                {
                    created.x = 8 * tileX;
                }
                if (canResize[1] && tileY < startTileY)
                {
                    created.y = 8 * tileY;
                }
                created.Resize(ghostEntity.width, ghostEntity.height);
            }
            else if (created.nodes.Count == 1)
            {
                created.nodes[0] = new(8 * tileX - 8 * startTileX, 8 * tileY - 8 * startTileY);
            }
            redrawGhost = true;

            backendRoom?.entities.Add(created);
            created.Draw();
        }

        public override void OnSelect()
        {
            NetworkManager.SendPacket(Netcode.MODIFY_ITEM_SHAPE, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", "cursorGhost"},
                {"index", 0},
                {"data", new JObject() {
                    {"visible", false}
                }}
            });
        }

        public override void OnDeselect()
        {
            NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", "cursorGhost"},
                {"action", "clear"}
            });

            NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", "cursorGhost"},
                {"data", new JObject() {
                    {"shapes", new JArray() {
                        new JObject() {
                            {"type", "tileGhost"},
                            {"color", "#aaaaaa"},
                            {"thickness", "@defer('@pen_thickness(\\'Mapping/MainView\\')')"},
                            {"width", 8},
                            {"height", 8},
                            {"depth", int.MinValue},
                            { "coords", new JArray() {
                                "0,0"
                            }}
                        }
                    }}
                }}
            });
            lastSelectedMaterial = "";
        }

        public override bool UpdateCursorGhost(float mouseX, float mouseY)
        {
            if (lastSelectedMaterial != selectedMaterial || redrawGhost)
            {
                EntityData found = CelesteModLoader.entities[selectedMaterial];
                NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
                {
                    {"widget", "Mapping/MainView"},
                    {"item", "cursorGhost"},
                    {"action", "clear"}
                });

                NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
                {
                    {"widget", "Mapping/MainView"},
                    {"item", "cursorGhost"},
                    {"data", new JObject() {
                        {"shapes", new JArray() {
                            new JObject() {
                                {"type", "tileGhost"},
                                {"color", "#aaaaaa"},
                                {"thickness", "@defer('@pen_thickness(\\'Mapping/MainView\\')')"},
                                {"width", 8},
                                {"height", 8},
                                {"depth", int.MinValue},
                                { "coords", new JArray() {
                                    "0,0"
                                }},
                                { "visible", false}
                            }
                        }}
                    }}
                });
                
                Dictionary<string, object> prevData = ghostEntity?.data ?? [];
                string prevName = ghostEntity?._name ?? "";
                EntityData prevEntityData = ghostEntity?.entityData;

                ghostEntity = Entity.DefaultFromData(found, RoomData.Default);

                if (redrawGhost && lastSelectedMaterial == selectedMaterial)
                {
                    ghostEntity.data = prevData;
                    ghostEntity._name = prevName;
                    ghostEntity.entityData = prevEntityData;
                }

                ghostEntity.Draw(0.5f, "cursorGhost", 1);
                lastSelectedMaterial = selectedMaterial;
                redrawGhost = false;
            }
            return dragging;
        }
    }
}