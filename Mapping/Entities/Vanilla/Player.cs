using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Player : CSEntityData
    {
        public override string EntityName => "player";

        public override List<string> PlacementNames()
        {
            return ["player"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>() {
                {"isDefaultSpawn", false}
            };
        }

        public override List<float> Justification(RoomData room, Entity entity) => [0.5f, 1.0f];
        public override string Texture(RoomData room, Entity entity) => "characters/player/sitDown00";
    }
}