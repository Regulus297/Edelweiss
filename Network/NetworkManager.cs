using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Network
{
    public static class NetworkManager
    {
        public static List<Packet> queued = [];

        public static void SendPacket(ulong code, object message)
        {
            queued.Add(new Packet(code, message.ToString()));
        }
        

        public static void DequeuePacket()
        {
            queued.RemoveAt(0);
        }

        public static void ReceivePacket(Packet packet)
        {
        }
    }
}