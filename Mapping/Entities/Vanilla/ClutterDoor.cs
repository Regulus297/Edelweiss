using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class ClutterDoor : CSEntityData
    {
        public override string EntityName => "clutterDoor";

        public override List<string> PlacementNames()
        {
            return ["red", "green", "yellow", "lightning"];
        }

        public override string FillColor(RoomData room, Entity entity) => EdelweissUtils.GetColor(74f / 255, 71f / 255, 135f / 255, 153f / 255f);
        public override string BorderColor(RoomData room, Entity entity) => "#ffffff";

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            string variant = (string)entity["type"];
            Rect rectangle = new(entity.x, entity.y, entity.width, entity.height, FillColor(room, entity), BorderColor(room, entity));
            Sprite sprite = new Sprite($"objects/resortclutter/icon_{variant}", entity);
            sprite.x += entity.width / 2;
            sprite.y += entity.height / 2;
            return [rectangle, sprite];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"type", placement},
                {"width", 24},
                {"height", 24}
            };
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            entity["type"] = PlacementNames().Cycle(entity["type"].ToString(), amount);
            return true;
        }

        public override JObject FieldInformation(string fieldName)
        {
            if (fieldName != "type")
                return null;
            return new JObject()
            {
                {"items", new JArray() {
                    "red", "green", "yellow", "lightning"
                }}
            };
        }
    }
}