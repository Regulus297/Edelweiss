using System;
using System.Collections.Generic;
using System.Linq;
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
            string tileData = room["shapes"][1]["tileData"].ToString();
            SetTile(ref tileData, room, tileX, tileY);
            NetworkManager.SendPacket(Netcode.MODIFY_ITEM_SHAPE, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", room["name"].ToString()},
                {"index", 1},
                {"data", new JObject() {
                    {"tileData", tileData}
                }}
            });
        }
    }
}