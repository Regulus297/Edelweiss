using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Edelweiss.Mapping.Entities;
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
        private int cursorGhostItems = 0;

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
                foreach (var entity in CelesteModLoader.entities)
                {
                    if (favourites.Contains(entity.Key))
                        continue;

                    if (IsSearched(entity.Value.DisplayName))
                        cachedMaterials[entity.Key] = entity.Value.DisplayName;
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
            else if (ghostEntity.nodes.Count > 0)
            {
                changed = true;
                ghostEntity.nodes[0] = new Point(tileX * 8 - startTileX * 8, tileY * 8 - startTileY * 8);
            }
            if (changed)
            {        
                NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
                {
                    {"widget", "Mapping/MainView"},
                    {"item", "cursorGhost"},
                    {"action", "clear"}
                });
                ghostEntity.Draw(0.5f, "cursorGhost", 1);
            }
        }

        public override void MouseRelease(JObject room, float x, float y)
        {
            dragging = false;
            RoomData backendRoom = MappingTab.map.rooms.FirstOrDefault(r => r.name == room.Value<string>("name"));
            EntityData found = CelesteModLoader.entities[selectedMaterial];
            (int tileX, int tileY) = EdelweissUtils.ToTileCoordinate(x, y);
            Entity created = Entity.DefaultFromData(found, backendRoom);
            created._name = found.Name;
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
            else if (created.nodes.Count > 0)
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
            NetworkManager.SendPacket(Netcode.MODIFY_ITEM_SHAPE, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", "cursorGhost"},
                {"index", 0},
                {"data", new JObject() {
                    {"visible", true}
                }}
            });

            NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", "cursorGhost"},
                {"action", "remove"},
                {"index", JToken.FromObject(Enumerable.Range(1, cursorGhostItems))}
            });
            cursorGhostItems = 0;
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
                                {"coords", new JArray() {
                                    "0,0"
                                }},
                                { "visible", false}
                            }
                        }}
                    }}
                });

                ghostEntity = Entity.DefaultFromData(found, RoomData.Default);
                ghostEntity.Draw(0.5f, "cursorGhost", 1);
                lastSelectedMaterial = selectedMaterial;
                redrawGhost = false;
            }
            return dragging;
        }
    }
}