using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class StrawberryBlockfield : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "blockField";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override string FillColor(RoomData room, Entity entity) => EdelweissUtils.GetColor(0.4f, 0.4f, 1.0f, 0.4f);
        public override string BorderColor(RoomData room, Entity entity) => EdelweissUtils.GetColor(0.4f, 0.4f, 1.0f, 1.0f);

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddResizability();
        }
    }
}