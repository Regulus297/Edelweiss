using System;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Network.PacketReceivers
{
    internal class RoomCreationReceiver : PluginPacketReceiver
    {
        public override long HandledCode => Netcode.FORM_SUBMITTED;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            int x = data.Value<JObject>("extraData").Value<int>("x");
            int y = data.Value<JObject>("extraData").Value<int>("y");

            Console.WriteLine(packet.data);

            JObject room = PluginLoader.RequestJObject("Edelweiss:GraphicsItems/room");
            room["x"] = x;
            room["y"] = y;
            NetworkManager.SendPacket(Netcode.ADD_ITEM, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", room}
            });
        }
    }
}