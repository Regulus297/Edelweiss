using Edelweiss.Network;
using Edelweiss.Plugins;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.PacketReceivers
{
    [LoadAfter(typeof(MappingTab))]
    internal class MouseMovedReceiver : PluginPacketReceiver
    {
        public override long HandledCode => MappingTab.MouseMovedNetcode;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            float mouseX = data.Value<float>("x");
            float mouseY = data.Value<float>("y");

            if (MappingTab.selectedTool?.UpdateCursorGhost(mouseX, mouseY) == true)
            {
                return;
            }

            (int x, int y) = EdelweissUtils.ToTileCoordinate(mouseX, mouseY);
            NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", "cursorGhost"},
                {"data", new JObject() {
                    {"x", x * 8},
                    {"y", y * 8}
                }}
            });
        }
    }
}