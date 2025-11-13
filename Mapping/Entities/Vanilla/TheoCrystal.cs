using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class TheoCrystal : CSEntityData
    {
        public override string EntityName => "theoCrystal";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override int Depth(RoomData room, Entity entity) => -100;
        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Sprite sprite = new Sprite("characters/theoCrystal/idle00", entity);
            sprite.y -= 10;
            return [sprite];
        }
    }
}