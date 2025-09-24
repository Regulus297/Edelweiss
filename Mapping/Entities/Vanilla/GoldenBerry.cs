using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class GoldenBerry : CSEntityData
    {
        public override string EntityName => "goldenBerry";

        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, -1];

        public override List<string> PlacementNames()
        {
            return ["golden", "golden_winged"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"winged", placement == "golden_winged"},
                {"moon", false}
            };
        }

        public override string Texture(RoomData room, Entity entity)
        {
            bool winged = (bool)entity["winged"];
            bool hasNodes = entity.nodes.Count > 0;

            string path = hasNodes ? "ghostgoldberry" : "goldberry";
            string sprite = winged ? "wings01" : "idle00";

            return $"collectables/{path}/{sprite}";
        }

        public override string NodeTexture(RoomData room, Entity entity, int nodeIndex)
        {
            return "collectables/goldberry/seed00";
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            entity["winged"] = !(bool)entity["winged"];
            return true;
        }
    }
}