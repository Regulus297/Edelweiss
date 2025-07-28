using System;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Network.PacketReceivers
{
    internal class BlahajButtonReceiver : PluginPacketReceiver
    {
        public override long HandledCode => Registry.registry[typeof(Plugin)].GetValue<MainPlugin>().NetcodeDynamic;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            string id = data.Value<string>("id");
            if (id == "blahajButton")
            {
                Console.WriteLine("Ha! this idiot thinks he/she/they is/are gonna get a blahaj!");
            }
            else if (id == "moreBlahajButton")
            {
                Console.WriteLine("Ha! this idiot wants even more blahaj!");
            }
        }
    }
}