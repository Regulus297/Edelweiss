using System;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Network.PacketReceivers
{
    public class TabChangeReceiver : PluginPacketReceiver
    {
        public override long HandledCode => Netcode.TAB_CHANGED;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            if (CustomTab.registeredTabs.TryGetValue(data.Value<string>("prev"), out CustomTab prevTab))
                prevTab.OnDeselect();
            if (CustomTab.registeredTabs.TryGetValue(data.Value<string>("curr"), out CustomTab currTab))
                currTab.Select();
        }
    }
}