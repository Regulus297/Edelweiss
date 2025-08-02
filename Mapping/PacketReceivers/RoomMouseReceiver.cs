using System;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.PacketReceivers
{
    [LoadAfter(typeof(MappingTab))]
    internal class RoomMouseReceiver : PluginPacketReceiver
    {
        public override long HandledCode => MappingTab.RoomMouseNetcode;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            float mouseX = data.Value<float>("x");
            float mouseY = data.Value<float>("y");
            string name = data.Value<string>("name");
            switch (data.Value<string>("type"))
            {
                case "press":
                    MappingTab.selectedTool?.MouseDown(name, mouseX, mouseY);
                    break;
                case "move":
                    MappingTab.selectedTool?.MouseDrag(name, mouseX, mouseY);
                    break;
                case "release":
                    MappingTab.selectedTool?.MouseRelease(name, mouseX, mouseY);
                    break;
            }
        }
    }
}