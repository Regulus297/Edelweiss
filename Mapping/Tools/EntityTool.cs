using System;
using System.Collections.Generic;
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
        private string lastSelectedMaterial = "";
        public override Dictionary<string, string> GetMaterials()
        {
            if (cachedMaterials == null)
            {
                cachedMaterials = [];
                foreach (var entity in CelesteModLoader.entities)
                {
                    cachedMaterials[entity.Key] = entity.Value.Name;
                }
            }
            return cachedMaterials;
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

            NetworkManager.SendPacket(Netcode.MODIFY_ITEM_SHAPE, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", "cursorGhost"},
                {"index", 1},
                {"data", new JObject() {
                    {"visible", true}
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

            NetworkManager.SendPacket(Netcode.MODIFY_ITEM_SHAPE, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", "cursorGhost"},
                {"index", 1},
                {"data", new JObject() {
                    {"visible", false}
                }}
            });
        }

        public override bool UpdateCursorGhost(float mouseX, float mouseY)
        {
            if (lastSelectedMaterial != selectedMaterial)
            {
                EntityData found = CelesteModLoader.entities[selectedMaterial];
                NetworkManager.SendPacket(Netcode.MODIFY_ITEM_SHAPE, new JObject()
                {
                    {"widget", "Mapping/MainView"},
                    {"item", "cursorGhost"},
                    {"index", 1},
                    {"data", new JObject() {
                        {"path", "Gameplay/" + found.Texture(RoomData.Default, Entity.Default)},
                        {"justification", JToken.FromObject(found.Justification(RoomData.Default, Entity.Default))}
                    }}
                });
                lastSelectedMaterial = selectedMaterial;
            }
            return false;
        }
    }
}