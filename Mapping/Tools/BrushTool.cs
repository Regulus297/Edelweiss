using System;
using System.Collections.Generic;
using System.Linq;
using Edelweiss.Network;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Tools
{
    public class BrushTool : MappingTool
    {
        public override List<string> Materials => MainPlugin.Instance.fgTiles.Select(t => t.Value.name).ToList();
        public override List<string> MaterialIDs => MainPlugin.Instance.fgTiles.Select(t => t.Value.ID).ToList();
        public override List<string> Layers => ["Foreground", "Background"];
        public override bool ClickingTriggersDrag => true;

        public override void MouseDrag(JObject room, float x, float y)
        {
            int tileX = (int)(x / 8);
            int tileY = (int)(y / 8);
            int i = tileY * ((int)room["width"] / 8) + tileX;
            string tileData = room["shapes"][1]["tileData"].ToString();
            tileData = tileData.Substring(0, i) + selectedMaterial + tileData.Substring(i + 1);
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