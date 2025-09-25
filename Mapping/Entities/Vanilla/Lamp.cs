using System.Collections.Generic;
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

            int width = sprite.atlasWidth / 2;
            sprite.x += -width / 2;
            sprite.y += -sprite.atlasHeight;
            sprite.sourceX = broken ? width : 0;
            sprite.sourceWidth = width;
            sprite.sourceHeight = sprite.atlasHeight;


            return [sprite];
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            return CycleBoolean(entity, "broken", amount);
        }

        // TODO: selection
    }
}