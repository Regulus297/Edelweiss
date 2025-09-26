using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class CrumbleWallOnRumble : CSEntityData
    {
        public override string EntityName => "crumbleWallOnRumble";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"tiletype", TileHelper.GetMaterial("m")},
                {"blendin", true},
                {"persistent", false},
                {"width", 8},
                {"height", 8}
            };
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity) => TileHelper.GetSprite(entity, "tiletype");
        public override int Depth(RoomData room, Entity entity) => (bool)entity["blendin"] ? -10501 : -12999;
        public override bool Cycle(RoomData room, Entity entity, int amount) => TileHelper.Cycle(entity, "tiletype", amount);
    }
}