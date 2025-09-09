using System.Collections.Generic;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.PacketReceivers
{
    [LoadAfter(typeof(MappingTab))]
    internal class MaterialSearchedReceiver : PacketReceiver
    {
        public override long HandledCode => MappingTab.MaterialSearchedNetcode;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            MappingTab.searchTerm = data.Value<string>("text").ToLower();
            MappingTab.materialIDs.Value = MappingTab.selectedTool.MaterialIDs;
            MappingTab.materials.Value = MappingTab.selectedTool.Materials;
        }
    }
}