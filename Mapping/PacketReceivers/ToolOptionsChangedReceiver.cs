using System;
using System.Collections.Generic;
using System.Diagnostics;
using Edelweiss.Mapping.Tools;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.PacketReceivers
{
    internal class ToolOptionsChangedReceiver : PluginPacketReceiver
    {
        public override long HandledCode => Netcode.LIST_SELECTION_CHANGED;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            switch (data["id"].ToString())
            {
                case "toolList":
                    ProcessToolChanged(data);
                    break;
                case "modeList":
                    ProcessModeChanged(data);
                    break;
                case "layerList":
                    ProcessLayerChanged(data);
                    break;
                case "materialList":
                    ProcessMaterialChanged(data);
                    break;
            }
        }

        private void ProcessToolChanged(JObject data)
        {
            string current = data["curr"].ToString();
            MappingTool t = (MappingTool)Registry.registry[typeof(MappingTool)].values.Find(t => ((MappingTool)t).DisplayName == current);
            MappingTab.selectedTool = t;
            if (t == null)
                return;

            MappingTab.layers.Value = t.Layers;
            MappingTab.modes.Value = t.Modes;
            MappingTab.materials.Value = t.Materials;

            MappingTab.selectedLayer.Value = t.selectedLayer;
            MappingTab.selectedMaterial.Value = t.selectedMaterial;
            MappingTab.selectedMode.Value = t.selectedMode;

            NetworkManager.SendPacket(Netcode.REFRESH_WIDGETS, new JObject()
            {
                {"widgets", JToken.FromObject(new List<string>() {"Mapping/ModeList", "Mapping/LayerList", "Mapping/MaterialList"})}
            });
        }

        private void ProcessModeChanged(JObject data)
        {
            if (MappingTab.selectedTool != null && data.Value<int>("currRow") >= 0)
            {
                MappingTab.selectedTool.selectedMode = data.Value<int>("currRow");
            }
        }

        private void ProcessLayerChanged(JObject data)
        {
            if (MappingTab.selectedTool != null && data.Value<int>("currRow") >= 0)
            {
                MappingTab.selectedTool.selectedLayer = data.Value<int>("currRow");
            }

        }
        
        private void ProcessMaterialChanged(JObject data)
        {
            if (MappingTab.selectedTool != null && data.Value<int>("currRow") >= 0)
            {
                MappingTab.selectedTool.selectedMaterial = data.Value<int>("currRow");
            }
            
        }
    }
}