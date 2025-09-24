using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Hahaha : CSEntityData
    {
        public override string EntityName => "hahaha";

        public override List<string> PlacementNames()
        {
            return ["hahaha"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"ifset", ""},
                {"triggerLaughSfx", false}
            };
        }

        public override int Depth(RoomData room, Entity entity) => -10001;
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override List<int> NodeLimits(RoomData room, Entity entity) => [1, 1];
        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            List<Drawable> sprites = [];

            List<(int, int)> offsets = [(-11, -1), (0, 0), (11, -1)];
            foreach ((int ox, int oy) in offsets)
            {
                Sprite sprite = new Sprite("characters/oldlady/ha00", entity);
                sprite.x += ox;
                sprite.y += oy;
                sprites.Add(sprite);
            }

            return sprites; 
        }
    }
}