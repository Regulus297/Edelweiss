using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Network.PacketReceivers
{
    internal class KeyPressReceiver : PacketReceiver
    {
        public override long HandledCode => Netcode.KEY_PRESSED;

        public override void ProcessPacket(Packet packet)
        {
            int key = (int)JObject.Parse(packet.data)["key"];
            Registry.ForAll<PluginSaveablePreference>(pref =>
            {
                if (pref is not PluginKeybind keybind || !keybind.CurrentBindings.Contains(key))
                    return;
                keybind.Press();
            });
        }
    }
}