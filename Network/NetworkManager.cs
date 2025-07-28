using System;
using System.Collections.Generic;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Network
{
    /// <summary>
    /// The class that handles sending and receiving packets on the backend
    /// </summary>
    public static class NetworkManager
    {
        /// <summary>
        /// The list of packets that are yet to be processed by the UI
        /// </summary>
        public static List<Packet> queued = [];

        /// <summary>
        /// Sends a packet to the UI
        /// </summary>
        /// <param name="code">The <see cref="Netcode"/> that the packet should have</param>
        /// <param name="message">The data that the packet should contain</param>
        public static void SendPacket(long code, object message)
        {
            queued.Add(new Packet(code, message.ToString()));
        }

        /// <summary>
        /// Removes the first packet from the queue
        /// </summary>
        public static void DequeuePacket()
        {
            queued.RemoveAt(0);
        }

        /// <summary>
        /// Called when the UI sends the backend a packet.
        /// </summary>
        /// <param name="packet">The received packet</param>
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