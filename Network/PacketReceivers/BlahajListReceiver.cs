using System;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Network.PacketReceivers
{
    internal class BlahajListReceiver : PluginPacketReceiver
    {
        public override long HandledCode => Netcode.LIST_SELECTION_CHANGED;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            if (data.Value<string>("id") == "blahajList")
            {
                object prev = data.Value<object>("prev");
                object curr = data.Value<object>("curr");
                Console.WriteLine($"Selected {curr}, deselected {prev}");
            }
        }
    }
}