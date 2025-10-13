using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class BadelineChaserBarrier : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "darkChaserEnd";

        public override List<string> PlacementNames()
        {
            return ["barrier"];
        }

        public override string FillColor(RoomData room, Entity entity) => EdelweissUtils.GetColor(0.4f, 0f, 0.4f, 0.4f);
        public override string BorderColor(RoomData room, Entity entity) => EdelweissUtils.GetColor(0.4f, 0, 0.4f, 1);

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddResizability();
        }
    }
}