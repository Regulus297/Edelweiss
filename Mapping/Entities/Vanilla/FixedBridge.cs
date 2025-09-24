using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class FixedBridge : CSEntityData
    {
        public override string EntityName => "bridgeFixed";

        public override List<string> PlacementNames()
        {
            return ["bridge_fixed"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"width", 32}
            };
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            int x = 0;
            List<Drawable> sprites = [];
            while (x < entity.width)
            {
                Sprite sprite = new Sprite("scenery/bridge_fixed", entity);
                sprite.x += x;
                sprite.y -= 8;

                x += sprite.data.width;
                sprites.Add(sprite);
            }
            return sprites;
        }
        
        // TODO: selection rect
    }
}