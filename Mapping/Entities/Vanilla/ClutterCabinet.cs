using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class ClutterCabinet : CSEntityData
    {
        public override string EntityName => "clutterCabinet";

        public override List<string> PlacementNames()
        {
            return ["cabinet"];
        }

        public override int Depth(RoomData room, Entity entity) => -10001;
        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Sprite sprite = new Sprite("objects/resortclutter/cabinet00", entity);
            sprite.x += 8;
            sprite.y += 8;
            sprite.justificationX = 0.5f;
            sprite.justificationY = 0.5f;
            return [sprite];
        }
    }
}