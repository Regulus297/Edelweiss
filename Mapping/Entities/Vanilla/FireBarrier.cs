using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class FireBarrier : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "fireBarrier";

        public override List<string> PlacementNames()
        {
            return ["fire_barrier"];
        }
        public override string FillColor(RoomData room, Entity entity) => EdelweissUtils.GetColor(209f / 255, 9f / 255, 1f / 255, 102f / 255);
        public override string BorderColor(RoomData room, Entity entity) => EdelweissUtils.GetColor(246f / 255, 98f / 255, 18f / 255);
        public override int Depth(RoomData room, Entity entity) => -8500;

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddResizability();
        }
    }
}