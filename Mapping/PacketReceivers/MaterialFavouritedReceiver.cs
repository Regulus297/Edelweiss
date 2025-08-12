using System;
using System.Collections.Generic;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.PacketReceivers
{
    [LoadAfter(typeof(MappingTab))]
    internal class MaterialFavouritedReceiver : PluginPacketReceiver
    {
        public override long HandledCode => MappingTab.MaterialFavouritedNetcode;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            MappingTab.selectedTool?.OnFavourited(data["itemID"].ToString());
            MappingTab.materialIDs.Value = MappingTab.selectedTool.MaterialIDs;
            MappingTab.materials.Value = MappingTab.selectedTool.Materials;
        }
    }
}