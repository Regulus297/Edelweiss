using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Strawberry : CSEntityData
    {
        public override string EntityName => "strawberry";

        public override List<string> PlacementNames()
        {
            return ["normal", "winged", "moon"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"winged", placement == "winged"},
                {"moon", placement == "moon"},
                {"checkpointID", -1},
                {"order", -1}
            };
        }

        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, -1];
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Fan;

        public override string Texture(RoomData room, Entity entity)
        {
            bool moon = entity.Get<bool>("moon");
            bool winged = entity.Get<bool>("winged");
            bool nodes = entity.nodes.Count > 0;

            return moon ? ((winged || nodes) ? "collectables/moonBerry/ghost00" : "collectables/moonBerry/normal00") : (winged ? (nodes ? "collectables/ghostberry/wings01" : "collectables/strawberry/wings01") : (nodes ? "collectables/ghostberry/idle00" : "collectables/strawberry/normal00"));
        }

        public override string NodeTexture(RoomData room, Entity entity, int nodeIndex)
        {
            return "collectables/strawberry/seed00";
        }

        public override int Depth(RoomData room, Entity entity) => -100;
    }
}