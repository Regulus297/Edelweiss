using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class StarClimbController : CSEntityData
    {
        public override string EntityName => "starClimbController";

        public override List<string> PlacementNames()
        {
            return [];
        }
        public override string Texture(RoomData room, Entity entity) => "@Internal@/northern_lights";
    }

    internal class EverestStarClimbController : CSEntityData
    {
        public override string EntityName => "everest/starClimbGraphicsController";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }
        public override string Texture(RoomData room, Entity entity) => "@Internal@/northern_lights";

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"fgColor", "#A3FFFF"},
                {"bgColor", "#293E4B"}
            };
        }

        public override List<string> Mods() => ["Everest"];
    }
}