using System.Collections.Generic;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.PacketReceivers
{
    [LoadAfter(typeof(MappingTab))]
    public class MaterialSearchedReceiver : PluginPacketReceiver
    {
        public override long HandledCode => MappingTab.MaterialSearchedNetcode;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            MappingTab.searchTerm = data.Value<string>("text").ToLower();
            MappingTab.materials.Value = MappingTab.selectedTool.Materials;
            MappingTab.materialIDs.Value = MappingTab.selectedTool.MaterialIDs;
            NetworkManager.SendPacket(Netcode.REFRESH_WIDGETS, new JObject()
            {
                {"widgets", JToken.FromObject(new List<string>() {"Mapping/MaterialList"})}
            });
        }
    }
}