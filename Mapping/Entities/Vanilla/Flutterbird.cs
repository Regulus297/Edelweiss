using System;
using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Flutterbird : CSEntityData
    {
        public override string EntityName => "flutterbird";

        private static readonly List<string> colors = [
            "#89fbff",
            "#f0fc6c",
            "#f493ff",
            "#93baff"
        ];

        public override List<string> PlacementNames()
        {
            return ["normal"];
        }

        public override int Depth(RoomData room, Entity entity) => -9999;

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Random random = new(entity.x * entity.y + room.x * room.y);
            Sprite sprite = new Sprite("scenery/flutterbird/idle00", entity);
            sprite.color = colors[random.Next(0, colors.Count)];
            sprite.justificationX = 0.5f;
            sprite.justificationY = 1.0f;
            return [sprite];
        }
    }
}