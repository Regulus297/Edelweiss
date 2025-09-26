using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class ExitBlock : CSEntityData
    {
        public override string EntityName => "exitBlock";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"tileType", TileHelper.GetMaterial()},
                {"playTransitionReveal", false},
                {"width", 8},
                {"height", 8}
            };
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity) => TileHelper.GetSprite(entity, "tileType", true, 0.7f);
        public override bool Cycle(RoomData room, Entity entity, int amount) => TileHelper.Cycle(entity, "tileType", amount);
        public override int Depth(RoomData room, Entity entity) => -13000;
    }
}