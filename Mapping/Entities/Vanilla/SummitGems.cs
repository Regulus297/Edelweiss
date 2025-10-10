using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class SummitGem : CSEntityData
    {
        public override string EntityName => "summitgem";

        public override List<string> PlacementNames()
        {
            return [];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"gem", 0}
            };
        }

        public override string Texture(RoomData room, Entity entity) => $"collectables/summitgems/{entity.Get("gem", 0)}/gem00";
    }
}