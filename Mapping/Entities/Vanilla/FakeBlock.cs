using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class FakeBlock : CSEntityData
    {
        public override string EntityName => "fakeBlock";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"tiletype", TileHelper.GetMaterial()},
                {"playTransitionReveal", false},
                {"width", 8},
                {"height", 8}
            };
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity) => TileHelper.GetSprite(entity, "tiletype", true, 0.7f);
        public override bool Cycle(RoomData room, Entity entity, int amount) => TileHelper.Cycle(entity, "tiletype", amount);
        public override int Depth(RoomData room, Entity entity) => -13000;
        public override JObject FieldInformation(string fieldName) => TileHelper.GetFieldInformation(fieldName, "tiletype");
    }
}