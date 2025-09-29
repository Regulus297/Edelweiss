using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class OshiroDoor : CSEntityData
    {
        public override string EntityName => "oshirodoor";

        public override List<string> PlacementNames()
        {
            return ["oshiro_door"];
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Rect rect = new Rect(entity.x, entity.y, 32, 32, EdelweissUtils.GetColor(74, 71, 135, 153), "#ffffff");
            Sprite oshiro = new Sprite("characters/oshiro/oshiro24", entity)
            {
                color = "#ccffffff"
            };
            oshiro.x += 16;
            oshiro.y += 16;

            return [rect, oshiro];
        }
    }
}