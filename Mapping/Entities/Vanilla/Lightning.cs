using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Lighting : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "lightning";

        public override List<string> PlacementNames()
        {
            return ["lightning"];
        }

        public override int Depth(RoomData room, Entity entity) => -1000100;
        public override string FillColor(RoomData room, Entity entity) => EdelweissUtils.GetColor(0.55f, 0.97f, 0.96f, 0.4f);
        public override string BorderColor(RoomData room, Entity entity) => EdelweissUtils.GetColor(0.99f, 0.96f, 0.47f, 1.0f);
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, 1];

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddResizability()
                .AddField("perLevel", false)
                .AddFloatField("moveTime", 5.0f, 0);
        }
    }
}