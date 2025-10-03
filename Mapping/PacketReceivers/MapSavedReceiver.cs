using System;
using Edelweiss.Mapping.SaveLoad;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.PacketReceivers
{   
    [LoadAfter(typeof(MappingTab))]
    internal class MapSavedReceiver : PacketReceiver
    {
        public override long HandledCode => MappingTab.SaveMapNetcode;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            try
            {
                MappingTab.filePath = data["path"].ToString();
                MapSaveLoad.SaveMap(MappingTab.map, MappingTab.filePath);
            }
            catch (Exception e)
            {
                MainPlugin.Instance.Logger.Error($"Error saving map: {e}");
            }
        }
    }
}