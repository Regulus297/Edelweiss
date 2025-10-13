using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class IceBlock : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "iceBlock";

        public override List<string> PlacementNames()
        {
            return ["ice_block"];
        }

        public override string FillColor(RoomData room, Entity entity) => EdelweissUtils.GetColor(76, 168, 214, 102);
        public override string BorderColor(RoomData room, Entity entity) => EdelweissUtils.GetColor(108, 214, 235);
        public override int Depth(RoomData room, Entity entity) => -8500;

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddResizability();
        }
    }
}