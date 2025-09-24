using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class FlingBird : CSEntityData
    {
        public override string EntityName => "flingBird";

        public override List<string> PlacementNames()
        {
            return ["fling_bird"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"waiting", false}
            };
        }

        public override int Depth(RoomData room, Entity entity) => -1;
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, -1];
        public override string Texture(RoomData room, Entity entity) => "characters/bird/Hover04";
    }
}