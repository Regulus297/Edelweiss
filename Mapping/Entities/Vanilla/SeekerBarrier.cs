using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class SeekerBarrier : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "seekerBarrier";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override string Color(RoomData room, Entity entity) => EdelweissUtils.GetColor(0.25f, 0.25f, 0.25f, 0.8f);

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddResizability();
        }
    }
}