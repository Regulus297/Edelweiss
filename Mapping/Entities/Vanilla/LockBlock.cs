using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class LockBlock : CSEntityData
    {
        public override string EntityName => "lockBlock";

        public override List<string> PlacementNames()
        {
            return ["wood", "temple_a", "temple_b", "moon"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"sprite", placement},
                {"unlock_sfx", ""},
                {"stepMusicProgress", false}
            };
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            string suffix = entity["sprite"].ToString() switch
            {
                "wood" => "00",
                "temple_a" => "TempleA00",
                "temple_b" => "TempleB00",
                _ => ""
            };

            string texture = suffix == "" ? "objects/door/moonDoor11" : $"objects/door/lockdoor{suffix}";

            Sprite sprite = new Sprite(texture, entity);
            sprite.x += 16;
            sprite.y += 16;

            return [sprite];
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            entity["sprite"] = PlacementNames().Cycle(entity["sprite"].ToString(), amount);
            return true;
        }

        public override JObject FieldInformation(string fieldName)
        {
            if (fieldName != "sprite")
                return null;
            return new JObject()
            {
                {"items", new JArray() {
                    "wood", "temple_a", "temple_b", "moon"
                }}
            };
        }
    }
}