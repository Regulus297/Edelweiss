using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class BadelineBoost : CSEntityData
    {
        public override string EntityName => "badelineBoost";
        public override int Depth(RoomData room, Entity entity)
        {
            return -1000000;
        }

        public override NodeLineRenderType NodeLineRenderType(Entity entity)
        {
            return Entities.NodeLineRenderType.Line;
        }

        public override string Texture(RoomData room, Entity entity)
        {
            return "objects/badelineboost/idle00";
        }

        public override List<int> NodeLimits(RoomData room, Entity entity)
        {
            return [0, -1];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>
            {
                {"lockCamera", true},
                {"canSkip", false},
                {"finalCh9Boost", false},
                {"finalCh9GoldenBoost", false},
                {"finalCh9Dialog", false}
            };
        }

        public override List<string> PlacementNames()
        {
            return ["boost"];
        }
    }
}