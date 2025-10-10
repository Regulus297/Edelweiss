using System;
using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class SummitCloud : CSEntityData
    {
        public override string EntityName => "summitcloud";

        public static readonly string[] Textures = [
            "scenery/summitclouds/cloud00",
            "scenery/summitclouds/cloud01",
            "scenery/summitclouds/cloud03",
        ];

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override int Depth(RoomData room, Entity entity) => -10550;
        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Random random = new(entity.x * entity.y + room.x * room.y);

            string texture = Textures[random.Next(Textures.Length)];
            Sprite sprite = new Sprite(texture, entity);
            float scaleX = random.Next(1) == 0 ? 1 : -1;
            sprite.scaleX = scaleX;

            return [sprite];
        }
    }
}