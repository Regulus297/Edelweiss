using System;
using System.Collections.Generic;
using System.Linq;
using Edelweiss.Mapping.Entities;
using Edelweiss.Mapping.Tools;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.PacketReceivers
{
    [LoadAfter(typeof(SelectionTool))]
    internal class SelectionRightClickReceiver : PacketReceiver
    {
        public override long HandledCode => SelectionTool.RightClickedNetcode;

        public override void ProcessPacket(Packet packet)
        {
            if (MappingTab.selectedTool is not SelectionTool)
                return;

            JObject data = JObject.Parse(packet.data);
            Entity found = MappingTab.GetEntity(data.Value<string>("name"));
            var others = SelectionTool.selected.Where(i => i.Item1.EntityName == found.EntityName && i.Item1._id != found._id).Select(i => i.Item1);
            JObject form = found.entityData?.GetFormTemplate(found, -1, others.ToArray());
            NetworkManager.SendPacket(Netcode.OPEN_POPUP_FORM, form);
        }
        
    }
}