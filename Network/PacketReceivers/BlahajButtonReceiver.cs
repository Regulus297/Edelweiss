using System;
using Edelweiss.Plugins;

namespace Edelweiss.Network.PacketReceivers
{
    internal class BlahajButtonReceiver : PluginPacketReceiver
    {
        public override long HandledCode => Netcode.BUTTON_PRESSED;

        public override void ProcessPacket(Packet packet)
        {
            if (packet.data == "blahajButton")
            {
                Console.WriteLine("Ha! this idiot thinks he/she/they is/are gonna get a blahaj!");
            }
        }
    }
}