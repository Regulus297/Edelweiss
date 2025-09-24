using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class BirdPath : CSEntityData
    {
        public override string EntityName => "birdPath";
        public override List<string> PlacementNames()
        {
            return ["bird_path"];
        }
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override string Texture(RoomData room, Entity entity) => "characters/bird/flyup00";
        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, -1];

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"only_once", false},
                {"onlyIfLeft", false},
                {"speedMult", 1.0f},
                {"angleFix", false},
                {"angleFixMaxRotation", 60.0f}
            };
        }
    }
}