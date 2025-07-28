using System;
using System.Collections.Generic;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Network
{
    public static class NetworkManager
    {
        public static List<Packet> queued = [];

        public static void SendPacket(long code, object message)
        {
            queued.Add(new Packet(code, message.ToString()));
        }
        

        public static void DequeuePacket()
        {
            queued.RemoveAt(0);
        }

        public static void ReceivePacket(Packet packet)
        {
            if (!PluginPacketReceiver.receivers.TryGetValue(packet.code, out var receivers))
            {
                Console.WriteLine($"No receivers for packet with code {packet.code} containing data {packet.data}");
                return;
            }

            foreach (PluginPacketReceiver receiver in receivers)
                {
                    receiver.ProcessPacket(packet);
                }
        }
    }
}