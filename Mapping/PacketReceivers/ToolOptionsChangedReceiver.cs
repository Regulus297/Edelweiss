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
            MappingTool t = (MappingTool)Registry.registry[typeof(MappingTool)].values.Find(t => ((MappingTool)t).FullName == data["currID"].ToString());
            MappingTab.selectedTool?.OnDeselect();
            MappingTab.selectedTool = t;
            t.OnSelect();
            if (t == null)
                return;

            MappingTab.layers.Value = t.Layers;
            MappingTab.modes.Value = t.Modes;
            MappingTab.materialIDs.Value = t.MaterialIDs;
            MappingTab.materials.Value = t.Materials;

            MappingTab.selectedLayer.Value = t.selectedLayer;
            MappingTab.selectedMaterial.Value = t.selectedMaterial;
            MappingTab.selectedMode.Value = t.selectedMode;
        }

        private void ProcessModeChanged(JObject data)
        {
            if (MappingTab.selectedTool != null && data.Value<int?>("currID") != null)
            {
                MappingTab.selectedTool.selectedMode = data.Value<int>("currID");
            }
        }

        private void ProcessLayerChanged(JObject data)
        {
            if (MappingTab.selectedTool != null && data.Value<int?>("currID") != null)
            {
                MappingTab.selectedTool.selectedLayer = data.Value<int>("currID");
                MappingTab.materialIDs.Value = MappingTab.selectedTool.MaterialIDs;
                MappingTab.materials.Value = MappingTab.selectedTool.Materials;
            }

        }
        
        private void ProcessMaterialChanged(JObject data)
        {
            if (MappingTab.selectedTool != null && data.Value<string>("currID") != null)
            {
                MappingTab.selectedTool.selectedMaterial = data.Value<string>("currID");
            }
            
        }
    }
}