using System.Collections.Generic;
using System.Drawing;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Key : CSEntityData
    {
        public override string EntityName => "key";

        public override List<string> PlacementNames()
        {
            return ["normal", "with_return"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            if (placement == "with_return")
            {
                return new Dictionary<string, object>()
                {
                    {"nodes", new List<Point>() {
                        {new Point(16, 0)},
                        {new Point(32, 0)}
                    }}
                };
            }
            return base.GetPlacementData();
        }

        public override int Depth(RoomData room, Entity entity) => -1000000;
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;

        public override string Texture(RoomData room, Entity entity) => "collectables/key/idle00";

        public override List<int> NodeLimits(RoomData room, Entity entity)
        {
            if (entity.nodes.Count > 0)
            {
                return [2, 2];
            }
            return [0, 0];
        }
    }
}