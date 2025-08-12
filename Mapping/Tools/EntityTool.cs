using System;
using System.Collections.Generic;
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
        public override Dictionary<string, string> GetMaterials()
        {
            if (cachedMaterials == null || previousSearchTerm != MappingTab.searchTerm || recache)
            {
                previousSearchTerm = MappingTab.searchTerm;
                recache = false;
                cachedMaterials = [];
                foreach (string material in favourites)
                {
                    if (IsSearched(CelesteModLoader.entities[material].Name))
                        cachedMaterials[material] = "★ " + CelesteModLoader.entities[material].Name;
                }
                foreach (var entity in CelesteModLoader.entities)
                {
                    if (favourites.Contains(entity.Key))
                        continue;
                        
                    if (IsSearched(entity.Value.Name))
                        cachedMaterials[entity.Key] = entity.Value.Name;
                }
            }
            return cachedMaterials;
        }

        public override void OnFavourited(string material)
        {
            favourites.Add(material);
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
            (int tileX, int tileY) = EdelweissUtils.ToTileCoordinate(x, y);
            JObject item = new()
            {
                {"name", selectedMaterial},
                {"x", 8 * tileX},
                {"y", 8 * tileY},
                {"width", 8},
                {"height", 8}
            };

            EntityData found = CelesteModLoader.entities[selectedMaterial];
            JArray shapes = new();
            found.Draw(shapes, RoomData.Default, Entity.DefaultFromData(found));

            item["shapes"] = shapes;

            NetworkManager.SendPacket(Netcode.ADD_ITEM, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"parent", room.Value<string>("name")},
                {"item", item}
            });
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
            if (lastSelectedMaterial != selectedMaterial)
            {
                EntityData found = CelesteModLoader.entities[selectedMaterial];
                NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
                {
                    {"widget", "Mapping/MainView"},
                    {"item", "cursorGhost"},
                    {"action", "remove"},
                    {"index", JToken.FromObject(Enumerable.Range(1, cursorGhostItems))}
                });

                JArray shapes = new();
                found.Draw(shapes, RoomData.Default, Entity.DefaultFromData(found));
                foreach (var shape in shapes)
                    shape["opacity"] = 0.5f;
                cursorGhostItems = shapes.Count;
                NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
                {
                    {"widget", "Mapping/MainView"},
                    {"item", "cursorGhost"},
                    {"index", 1},
                    {"action", "add"},
                    { "shapes", shapes}
                });
                lastSelectedMaterial = selectedMaterial;
            }
            return false;
        }
    }
}