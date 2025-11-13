using System;
using System.Collections.Generic;
using Edelweiss.Mapping.Entities;
using Edelweiss.Mapping.Tools;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.PacketReceivers
{
    [LoadAfter(typeof(SelectionTool))]
    internal class SelectionResizedReceiver : PacketReceiver
    {
        public override long HandledCode => SelectionTool.SelectionResizedNetcode;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            int x = data.Value<int>("deltaWidth");
            int y = data.Value<int>("deltaHeight");
            if (x == 0 && y == 0)
                return;

            int x0 = data.Value<int>("oldWidth");
            int y0 = data.Value<int>("oldHeight");


            string id = data.Value<string>("name");
            Entity entity = MappingTab.GetEntity(id);
            if (entity == null)
                return;

            entity.Resize(x0 + x, y0 + y, 8);
            entity.entityRoom?.RedrawEntities();
        }
    }
}