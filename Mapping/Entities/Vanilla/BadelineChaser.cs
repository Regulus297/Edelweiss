using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class BadelineChaser : CSEntityData
    {
        public override string EntityName => "darkChaser";

        public override List<string> PlacementNames()
        {
            return ["dark_chaser"];
        }

        public override List<float> Justification(RoomData room, Entity entity) => [0.5f, 1.0f];
        public override string Texture(RoomData room, Entity entity) => "characters/badeline/sleep00";
        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>
            {
                {"canChangeMusic", true}
            };
        }
    }
}