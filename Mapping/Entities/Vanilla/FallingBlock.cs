using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class FallingBlock : CSEntityData
    {
        public override string EntityName => "fallingBlock";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"tiletype", TileHelper.GetMaterial()},
                {"width", 8},
                {"height", 8},
                {"climbFall", true},
                {"behind", false}
            };
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity) => TileHelper.GetSprite(entity, "tiletype");
        public override int Depth(RoomData room, Entity entity) => entity.Get<bool>("behind") ? 5000 : 0;
        public override bool Cycle(RoomData room, Entity entity, int amount) => TileHelper.Cycle(entity, "tiletype", amount);
        public override JObject FieldInformation(string fieldName) => TileHelper.GetFieldInformation(fieldName, "tiletype");
    }
}