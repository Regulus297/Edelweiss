using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class SummitBackgroundManager : CSEntityData
    {
        public override string EntityName => "SummitBackgroundManager";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"index", 0},
                {"cutscene", ""},
                {"intro_launch", false},
                {"dark", false},
                {"ambience", ""}
            };
        }

        public override string Texture(RoomData room, Entity entity) => "@Internal@/summit_background_manager";
    }
}