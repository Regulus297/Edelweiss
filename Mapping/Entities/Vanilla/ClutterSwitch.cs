using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class ClutterSwitch : CSEntityData
    {
        public override string EntityName => "colorSwitch";

        public override List<string> PlacementNames()
        {
            return ["red", "green", "yellow", "lightning"];
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Sprite buttonSprite = new Sprite("objects/resortclutter/clutter_button00", entity);
            Sprite clutterSprite = new Sprite($"objects/resortclutter/icon_{entity.Get("type", "red")}", entity);

            buttonSprite.x += 16;
            buttonSprite.y += 16;
            buttonSprite.justificationX = 0.5f;
            buttonSprite.justificationY = 1.0f;

            clutterSprite.x += 16;
            clutterSprite.y += 8;
            clutterSprite.justificationX = 0.5f;
            clutterSprite.justificationY = 0.5f;

            return [buttonSprite, clutterSprite];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"type", placement}
            };
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            entity["type"] = PlacementNames().Cycle(entity.Get("type", "red"), amount);
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