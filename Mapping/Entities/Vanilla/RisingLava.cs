using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class RisingLava : CSEntityData
    {
        public override string EntityName => "risingLava";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"intro", false}
            };
        }

        public override string Texture(RoomData room, Entity entity) => "@Internal@/rising_lava";
    }
}