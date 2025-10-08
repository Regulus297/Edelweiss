using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Lamp : CSEntityData
    {
        public override string EntityName => "lamp";

        public override List<string> PlacementNames()
        {
            return ["normal", "broken"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"broken", placement == "broken"}
            };
        }

        public override int Depth(RoomData room, Entity entity) => 5;
        public override List<float> Justification(RoomData room, Entity entity) => [0.25f, 0.25f];

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            bool broken = (bool)entity["broken"];

            Sprite sprite = new Sprite("scenery/lamp", entity)
            {
                justificationX = 0,
                justificationY = 0
            };

            // TODO: Fix source rect implementation to make these things line up
            sprite.x -= sprite.atlasWidth / 4;
            sprite.y -= sprite.atlasHeight;
            sprite.sourceX = broken ? sprite.atlasWidth / 2  : 0;
            sprite.sourceWidth = sprite.atlasWidth / 2;
            sprite.sourceHeight = sprite.atlasHeight;


            return [sprite];
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            return CycleBoolean(entity, "broken", amount);
        }

        public override List<Rectangle> Selection(RoomData room, Entity entity)
        {
            Sprite sprite = new Sprite("scenery/lamp", entity)
            {
                justificationX = 0,
                justificationY = 0
            };
            return [new Rectangle(entity.x - sprite.atlasWidth / 4, entity.y - sprite.atlasHeight, sprite.atlasWidth / 2, sprite.atlasHeight)];
        }
    }
}