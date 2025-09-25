using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class PowerSourceNumber : CSEntityData
    {
        public override string EntityName => "powerSourceNumber";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }
        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"number", 1},
                {"straberries", ""},
                {"keys", ""}
            };
        }

        public override int Depth(RoomData room, Entity entity) => -10010;

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Sprite number = new Sprite("scenery/powersource_numbers/1");
            Sprite glow = new Sprite("scenery/powersource_numbers/1_glow");
            return [number, glow];
        }
    }
}