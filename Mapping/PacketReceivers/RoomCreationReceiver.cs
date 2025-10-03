using System;
using System.Collections.Generic;
using System.Linq;
using Edelweiss.Mapping.Entities;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.PacketReceivers
{
    internal class RoomCreationReceiver : PacketReceiver
    {
        public override long HandledCode => Netcode.FORM_SUBMITTED;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            if (data.Value<string>("id") != "RoomCreationForm")
                return;

            JObject extraData = data.Value<JObject>("extraData");
            int x = extraData.Value<int>("x");
            int y = extraData.Value<int>("y");
            int width = extraData.Value<int>("width");
            int height = extraData.Value<int>("height");
            string color = extraData.Value<string>("colour");


            JObject room = PluginLoader.RequestJObject("Edelweiss:GraphicsItems/room");
            room["x"] = x * 8;
            room["y"] = y * 8;
            room["width"] = width * 8;
            room["height"] = height * 8;
            room["shapes"][2]["color"] = GetColor(color);
            room["shapes"][0]["tileData"] = string.Concat(Enumerable.Repeat(" ", width * height));
            room["shapes"][0]["width"] = width;
            room["shapes"][0]["height"] = height;
            room["shapes"][1]["tileData"] = string.Concat(Enumerable.Repeat(" ", width * height));
            room["shapes"][1]["width"] = width;
            room["shapes"][1]["height"] = height;
            room["name"] = extraData.Value<string>("name");

            MappingTab.map.rooms.Add(new RoomData(extraData)
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

        private string GetColor(string choice)
        {
            return choice switch
            {
                "1" => "#ed6a1f",
                "2" => "#88e33d",
                "3" => "#3be3dd",
                "4" => "#227ac7",
                "5" => "#a723b0",
                "6" => "#d61aa1",
                _ => "#ffffff"
            };
        }
    }
}