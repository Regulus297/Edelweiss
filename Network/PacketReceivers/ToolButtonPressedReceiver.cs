using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Network.PacketReceivers
{
    internal class ToolButtonPressedReceiver : PluginPacketReceiver
    {
        public override long HandledCode => Netcode.TOOL_BUTTON_PRESSED;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            if (!CustomTab.registeredTabs.TryGetValue(data.Value<string>("tab"), out CustomTab tab))
                return;

            tab.HandleToolbarClick(data.Value<string>("name"), data.Value<JObject>("extraData"));
        }
    }
}