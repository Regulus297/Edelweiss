using System;
using System.Collections.Generic;
using Edelweiss.Mapping.Tools;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.PacketReceivers
{
    internal class SelectedToolChangedReceiver : PluginPacketReceiver
    {
        public override long HandledCode => Netcode.LIST_SELECTION_CHANGED;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            if (data["id"].ToString() != "toolList")
                return;

            string current = data["curr"].ToString();
            MappingTool t = (MappingTool)Registry.registry[typeof(MappingTool)].values.Find(t => ((MappingTool)t).DisplayName == current);
            if (t == null)
                return;

            MappingTab.layers.Value = t.Layers;
            MappingTab.modes.Value = t.Modes;
            MappingTab.materials.Value = t.Materials;

            NetworkManager.SendPacket(Netcode.REFRESH_WIDGETS, new JObject()
            {
                {"widgets", JToken.FromObject(new List<string>() {"Mapping/ModeList", "Mapping/LayerList", "Mapping/MaterialList"})}
            });
        }
    }
}