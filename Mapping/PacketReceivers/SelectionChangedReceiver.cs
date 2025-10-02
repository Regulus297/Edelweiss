using System.Collections.Generic;
using Edelweiss.Mapping.Entities;
using Edelweiss.Mapping.Tools;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.PacketReceivers
{
    [LoadAfter(typeof(SelectionTool))]
    internal class SelectionChangedReceiver : PacketReceiver
    {
        public override long HandledCode => SelectionTool.SelectionChangedNetcode;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            Entity found = MappingTab.map.allEntities.GetValueOrDefault(data.Value<string>("name"));
            if (found == null)
                return;

            int index = data.Value<int>("index");

            if (data.Value<bool>("selected"))
            {
                SelectionTool.selected.Add((found, index));
            }
            else
            {
                SelectionTool.selected.Remove((found, index));
            }
        }
    }
}