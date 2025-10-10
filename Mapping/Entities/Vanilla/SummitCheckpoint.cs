using System;
using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class SummitCheckpoint : CSEntityData
    {
        public override string EntityName => "summitcheckpoint";
        public override int Depth(RoomData room, Entity entity) => 8999;

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"number", 0}
            };
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            int number = entity.Get("number", 0);
            int digit1 = number % 100 / 10;
            int digit2 = number % 10;

            Sprite back = new Sprite("scenery/summitcheckpoints/base02", entity);
            Sprite backDigit1 = new Sprite($"scenery/summitcheckpoints/numberbg0{digit1}", entity);
            Sprite frontDigit1 = new Sprite($"scenery/summitcheckpoints/number0{digit1}", entity);
            Sprite backDigit2 = new Sprite($"scenery/summitcheckpoints/numberbg0{digit2}", entity);
            Sprite frontDigit2 = new Sprite($"scenery/summitcheckpoints/number0{digit2}", entity);

            backDigit1.x -= 2;
            backDigit1.y += 4;
            frontDigit1.x -= 2;
            frontDigit1.y += 4;
            backDigit2.x += 2;
            backDigit2.y += 4;
            frontDigit2.x += 2;
            frontDigit2.y += 4;

            return [back, backDigit1, backDigit2, frontDigit1, frontDigit2];
        }
    }
}