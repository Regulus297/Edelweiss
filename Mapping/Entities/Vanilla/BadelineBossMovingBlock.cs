using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class BadelineBossMovingBlock : CSEntityData
    {
        public override string EntityName => "finalBossMovingBlock";

        public override List<int> NodeLimits(RoomData room, Entity entity) => [1, 1];
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"nodeIndex", 0},
                {"width", 8},
                {"height", 8}
            };
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            return [new Tiles("G", true, entity.x, entity.y, entity.width / 8, entity.height / 8)];
        }

        public override List<Drawable> NodeSprite(RoomData room, Entity entity, int nodeIndex)
        {
            Point node = entity.GetNode(nodeIndex);
            return [new Tiles("g", true, node.X, node.Y, entity.width / 8, entity.height / 8)];
        }
    }
}