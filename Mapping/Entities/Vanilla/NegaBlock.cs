using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class NegaBlock : CSEntityData
    {
        public override string EntityName => "negaBlock";

        public override string Color(RoomData room, Entity entity) => "#ff0000";

        public override List<string> PlacementNames()
        {
            return ["nega_block"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"width", 8},
                {"height", 8}
            };
        }
    }
}