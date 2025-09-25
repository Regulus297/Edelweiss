using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class PlayerSeeker : CSEntityData
    {
        public override string EntityName => "playerSeeker";

        public override List<string> PlacementNames()
        {
            return ["player_seeker"];
        }

        public override string Texture(RoomData room, Entity entity) => "decals/5-temple/statue_e";
    }
}