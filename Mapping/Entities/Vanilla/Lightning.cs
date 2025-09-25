using System.Collections.Generic;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Lighting : CSEntityData
    {
        public override string EntityName => "lightning";

        public override List<string> PlacementNames()
        {
            return ["lightning"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"width", 8},
                {"height", 8},
                {"perLevel", false},
                {"moveTime", 5.0f}
            };
        }

        public override int Depth(RoomData room, Entity entity) => -1000100;
        public override string FillColor(RoomData room, Entity entity) => EdelweissUtils.GetColor(0.55f, 0.97f, 0.96f, 0.4f);
        public override string BorderColor(RoomData room, Entity entity) => EdelweissUtils.GetColor(0.99f, 0.96f, 0.47f, 1.0f);
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, 1];
    }
}