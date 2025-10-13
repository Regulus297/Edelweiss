using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

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
        public override void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("crashes", false);
        }
    }
}