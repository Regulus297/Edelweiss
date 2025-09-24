using System;
using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class DreamBlock : CSEntityData
    {
        public override string EntityName => "dreamBlock";

        public override string FillColor(RoomData room, Entity entity) => "#000000";
        public override string BorderColor(RoomData room, Entity entity) => "#ffffff";
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, 1];

        public override List<string> PlacementNames()
        {
            return ["dream_block"];
        }

        public override int Depth(RoomData room, Entity entity)
        {
            return (bool)entity["below"] ? 5000 : -11000;
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            entity["below"] = !(bool)entity["below"];
            return true;
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"fastMoving", false},
                {"below", false},
                {"oneUse", false},
                {"width", 8},
                {"height", 8}
            };
        }
    }
}