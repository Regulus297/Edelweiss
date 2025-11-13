using System;
using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Entities;
using Edelweiss.Mapping.Tools;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.PacketReceivers
{
    [LoadAfter(typeof(SelectionTool))]
    internal class SelectionMovedReceiver : PacketReceiver
    {
        public override long HandledCode => SelectionTool.SelectionMovedNetcode;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            int x = data.Value<int>("x");
            int y = data.Value<int>("y");
            if (x == 0 && y == 0)
                return;

            string id = data.Value<string>("name");
            Entity entity = MappingTab.GetEntity(id);
            if (entity == null)
                return;

            int index = data.Value<int>("index");
            if (index == 0)
            {
                entity.x += x;
                entity.y += y;
            }
            else
            {
                Point node = entity.nodes[index - 1];
                entity.nodes[index - 1] = new Point(node.X + x, node.Y + y);
            }
            entity.entityRoom?.RedrawEntities();
        }
    }
}