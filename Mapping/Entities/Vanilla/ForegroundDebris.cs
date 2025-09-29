using System;
using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class ForegroundDebris : CSEntityData
    {
        public override string EntityName => "foregroundDebris";

        private static readonly List<List<string>> textures = [
            ["a00", "a01", "a02"],
            ["b00", "b01"]
        ];

        public override List<string> PlacementNames()
        {
            return ["foreground_debris"];
        }

        public override int Depth(RoomData room, Entity entity) => -999900;
        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            List<Drawable> sprites = [];

            Random random = new(entity.x * entity.y + room.x * room.y);
            foreach (string texture in textures[random.Next(0, textures.Count)])
            {
                sprites.Add(new Sprite("scenery/fgdebris/rock_" + texture, entity));
            }

            return sprites;
        }

        public override List<Rectangle> Selection(RoomData room, Entity entity) => [new Rectangle(entity.x - 24, entity.y - 24, 48, 48)];
    }
}