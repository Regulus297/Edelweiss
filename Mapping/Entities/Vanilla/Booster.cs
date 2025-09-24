using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Booster : CSEntityData
    {
        public override string EntityName => "booster";

        public override List<string> PlacementNames()
        {
            return ["green", "red"];
        }

        public override int Depth(RoomData room, Entity entity) => -8500;
        public override string Texture(RoomData room, Entity entity)
        {
            bool red = (bool)entity.data["red"];
            return red ? "objects/booster/boosterRed00" : "objects/booster/booster00";
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"red", placement == "red"},
                {"ch9_hub_booster", false}
            };
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            entity.data["red"] = !(bool)entity.data["red"];
            return true;
        }
    }
}