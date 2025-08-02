using System;
using System.Collections.Generic;
using System.Linq;
using Edelweiss.Network;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Tools
{
    internal class RectTool : TileTool
    {
        private bool isUsing;
        private int startX, startY;
        private int startRoomX, startRoomY;
        public override List<string> Modes => ["Fill", "Line"];

        public override void MouseClick(JObject room, float x, float y)
        {
            isUsing = true;
            (startX, startY) = EdelweissUtils.ToTileCoordinate(x + room.Value<float>("x"), y + room.Value<float>("y"));
            (startRoomX, startRoomY) = EdelweissUtils.ToTileCoordinate(x, y);
        }

        public override void MouseRelease(JObject room, float x, float y)
        {
            isUsing = false;

            // Paint tiles
            (int currentX, int currentY) = EdelweissUtils.ToTileCoordinate(x, y);
            int dirX = startRoomX < currentX ? 1 : -1;
            int dirY = startRoomY < currentY ? 1 : -1;
            string tileData = room["shapes"][1]["tileData"].ToString();
            for (int loopX = 0; Math.Abs(loopX) <= Math.Abs(currentX - startRoomX); loopX += dirX)
            {
                for (int loopY = 0; Math.Abs(loopY) <= Math.Abs(currentY - startRoomY); loopY += dirY)
                {
                    if (selectedMode == 1 && !(loopX == 0 || loopY == 0 || Math.Abs(loopX) == Math.Abs(currentX - startRoomX) || Math.Abs(loopY) == Math.Abs(currentY - startRoomY)))
                            continue;
                    SetTile(ref tileData, room, loopX + startRoomX, loopY + startRoomY);
                }
            }
            NetworkManager.SendPacket(Netcode.MODIFY_ITEM_SHAPE, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", room["name"].ToString()},
                {"index", 1},
                {"data", new JObject() {
                    {"tileData", tileData}
                }}
            });

            // Reset cursor ghost to original state
            NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", "cursorGhost"},
                {"data", new JObject() {
                    {"width", 8},
                    {"height", 8}
                }}
            });

            NetworkManager.SendPacket(Netcode.MODIFY_ITEM_SHAPE, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", "cursorGhost"},
                {"index", 0},
                {"data", new JObject() {
                    {"coords", JToken.FromObject(new List<string>() {"0,0"})}
                }}
            });
        }

        public override bool UpdateCursorGhost(float mouseX, float mouseY)
        {
            if (isUsing)
            {
                (int currentX, int currentY) = EdelweissUtils.ToTileCoordinate(mouseX, mouseY);
                List<string> coords = [];

                int dirX = startX < currentX ? 1 : -1;
                int dirY = startY < currentY ? 1 : -1;
                for (int i = 0; Math.Abs(i) <= Math.Abs(currentX - startX); i += dirX)
                {
                    for (int j = 0; Math.Abs(j) <= Math.Abs(currentY - startY); j += dirY)
                    {
                        if (selectedMode == 1 && !(i == 0 || j == 0 || Math.Abs(i) == Math.Abs(currentX - startX) || Math.Abs(j) == Math.Abs(currentY - startY)))
                            continue;
                        coords.Add($"{i},{j}");
                    }
                }

                NetworkManager.SendPacket(Netcode.MODIFY_ITEM_SHAPE, new JObject()
                {
                    {"widget", "Mapping/MainView"},
                    {"item", "cursorGhost"},
                    {"index", 0},
                    {"data", new JObject() {
                        {"coords", JToken.FromObject(coords)}
                    }}
                });
            }
            return isUsing;
        }
    }
}