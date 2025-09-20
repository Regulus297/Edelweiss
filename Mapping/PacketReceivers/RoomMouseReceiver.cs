using System;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.PacketReceivers
{
    [LoadAfter(typeof(MappingTab))]
    internal class RoomMouseReceiver : PacketReceiver
    {
        public override long HandledCode => MappingTab.RoomMouseNetcode;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);

            if (data.Value<int>("button") > 1)
                return;

            float mouseX = data.Value<float>("x");
            float mouseY = data.Value<float>("y");
            JObject room = (JObject) data["item"];
            switch (data.Value<string>("type"))
            {
                case "press":
                    MappingTab.selectedTool?.MouseDown(room, mouseX, mouseY);
                    break;
                case "move":
                    MappingTab.selectedTool?.MouseDrag(room, mouseX, mouseY);
                    break;
                case "release":
                    MappingTab.selectedTool?.MouseRelease(room, mouseX, mouseY);
                    break;
            }
        }
    }
}