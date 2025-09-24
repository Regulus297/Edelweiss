using System;
using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class FloatingDebris : CSEntityData
    {
        public override string EntityName => "floatingDebris";

        public override List<string> PlacementNames()
        {
            return ["floating_debris"];
        }

        public override int Depth(RoomData room, Entity entity) => -5;

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Random random = new(entity.x * entity.y + room.x * room.y);
            Sprite sprite = new Sprite("scenery/debris", entity);
            sprite.sourceX = random.Next(0, 7) * 8;
            sprite.sourceY = 0;
            sprite.sourceWidth = sprite.sourceHeight = 8;
            sprite.justificationX = sprite.justificationY = 0f;
            sprite.x += -4;
            sprite.y += -4;
            return [sprite];
        }

        // TODO: selection rect
    }
}