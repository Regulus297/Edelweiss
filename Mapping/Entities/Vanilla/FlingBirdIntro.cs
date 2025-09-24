using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class FlingBirdIntro : FlingBird
    {
        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"crashes", false}
            };
        }

        public override int Depth(RoomData room, Entity entity) => 0;
        public override List<int> NodeLimits(RoomData room, Entity entity) => [1, -1];
    }
}