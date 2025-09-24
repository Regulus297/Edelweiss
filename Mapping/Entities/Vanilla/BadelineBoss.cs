using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class BadelineBoss : CSEntityData
    {
        public override string EntityName => "finalBoss";

        public override List<string> PlacementNames()
        {
            return ["boss"];
        }


        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override string Texture(RoomData room, Entity entity) => "characters/badelineBoss/charge00";
        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, -1];
        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>
            {
                {"patternIndex", 1},
                {"startHit", false},
                {"cameraPastY", 120.0},
                {"cameraLockY", true},
                {"canChangeMusic", true}
            };
        }
    }
}