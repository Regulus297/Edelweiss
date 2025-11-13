using System;
using System.Collections.Generic;
using System.Linq;
using Edelweiss.Mapping.Entities;
using Edelweiss.Network;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Tools
{
    internal class BrushTool : TileTool
    {
        public override bool ClickingTriggersDrag => true;

        public override void MouseDrag(JObject room, float x, float y)
        {
            int tileX = (int)(x / 8);
            int tileY = (int)(y / 8);
            string tileData = room["shapes"][1-selectedLayer]["tileData"].ToString();
            RoomData backendRoom = MappingTab.map.rooms.Find(r => r.name == room["name"].ToString());
            SetTile(ref tileData, room, tileX, tileY);
            if(selectedLayer == 0)
                backendRoom.fgTileData.SetTile(tileX, tileY, selectedMaterial);
            else
                backendRoom.bgTileData.SetTile(tileX, tileY, selectedMaterial);
            NetworkManager.SendPacket(Netcode.MODIFY_ITEM_SHAPE, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", room["name"].ToString()},
                {"index", 1-selectedLayer},
                {"data", new JObject() {
                    {"tileData", tileData}
                }}
            });
        }
    }
}