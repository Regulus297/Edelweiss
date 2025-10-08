using System.Collections.Generic;
using Newtonsoft.Json.Linq;

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

        public override JObject FieldInformation(string fieldName)
        {
            if (fieldName != "patternIndex")
                return null;

            return new JObject()
            {
                {"fieldType", "integer"},
                {"items", new JArray() {
                    0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15
                }}
            };
        }
    }
}