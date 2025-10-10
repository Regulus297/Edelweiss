using System;
using System.Collections.Generic;
using System.Linq;
using Edelweiss.Mapping.Entities;
using Edelweiss.Mapping.Tools;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.PacketReceivers
{
    internal class MappingFormReceiver : PacketReceiver
    {
        public override long HandledCode => Netcode.FORM_SUBMITTED;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            string id = data.Value<string>("id");
            JObject extraData = data.Value<JObject>("extraData");
            if (id == "RoomCreationForm")
                HandleRoomCreation(extraData);
            else if (id == "MapMetaForm")
                HandleMetaEdited(extraData);
            else if (id.StartsWith("EntityData : "))
                HandleEntityDataEdited(id.Substring("EntityData : ".Count()), extraData);
        }

        private void HandleEntityDataEdited(string entityID, JObject data)
        {
            if (MappingTab.selectedTool is not SelectionTool)
                return;

            Entity found = MappingTab.map.allEntities.GetValueOrDefault(entityID);
            var others = SelectionTool.selected.Where(i => i.Item1.EntityName == found.EntityName && i.Item1._id != found._id).Select(i => i.Item1);
            found.UpdateData(data, -1, others.ToArray());
            found.entityRoom.RedrawEntities();
        }

        private void HandleMetaEdited(JObject data)
        {
            MapMeta meta = MappingTab.map.meta;
            meta.inventory = (MapMeta.Inventory)Enum.Parse(typeof(MapMeta.Inventory), data.Value<string>("inventory"));
            meta.theoInBubble = data.Value<bool>("theoInBubble");
            meta.seekerSlowdown = data.Value<bool>("seekerSlowdown");
            meta.heartIsEnd = data.Value<bool>("heartIsEnd");
            meta.dreaming = data.Value<bool>("dreaming");
            meta.interlude = data.Value<bool>("interlude");
            meta.overrideASideMeta = data.Value<bool>("overrideASideMeta");
            meta.bloomBase = data.Value<float>("bloomBase");
            meta.bloomStrength = data.Value<float>("bloomStrength");
            meta.darknessAlpha = data.Value<float>("darknessAlpha");
            meta.wipe = data.Value<string>("wipe");
            meta.colourGrade = data.Value<string>("colourGrade");
            meta.introType = (MapMeta.IntroType)Enum.Parse(typeof(MapMeta.IntroType), data.Value<string>("introType"));
        }

        private void HandleRoomCreation(JObject data)
        {
            int x = data.Value<int>("x");
            int y = data.Value<int>("y");
            int width = data.Value<int>("width");
            int height = data.Value<int>("height");
            string color = data.Value<string>("colour");


            JObject room = PluginLoader.RequestJObject("Edelweiss:GraphicsItems/room");
            room["x"] = x * 8;
            room["y"] = y * 8;
            room["width"] = width * 8;
            room["height"] = height * 8;
            room["shapes"][2]["color"] = RoomData.GetColor(color);
            room["shapes"][0]["tileData"] = string.Concat(Enumerable.Repeat(" ", width * height));
            room["shapes"][0]["width"] = width;
            room["shapes"][0]["height"] = height;
            room["shapes"][1]["tileData"] = string.Concat(Enumerable.Repeat(" ", width * height));
            room["shapes"][1]["width"] = width;
            room["shapes"][1]["height"] = height;
            room["name"] = data.Value<string>("name");

            MappingTab.map.rooms.Add(new RoomData(data)
            {
                map = MappingTab.map,
                fgTileData = room["shapes"][1]["tileData"].ToString(),
                bgTileData = room["shapes"][0]["tileData"].ToString()
            });
            MappingTab.rooms.Value = MappingTab.map.rooms.Select(r => r.name);

            NetworkManager.SendPacket(Netcode.ADD_ITEM, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", room}
            });
        }
    }
}