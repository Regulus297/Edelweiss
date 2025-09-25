using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class MoonCreature : CSEntityData
    {
        public override string EntityName => "moonCreature";

        public override List<string> PlacementNames()
        {
            return ["moon_creature"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"number", 1}
            };
        }

        public override int Depth(RoomData room, Entity entity) => -1000000;
        public override string Texture(RoomData room, Entity entity) => "scenery/moon_creatures/tiny05";
    }
}