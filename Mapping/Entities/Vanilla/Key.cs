using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Key : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "key";

        public override List<string> PlacementNames()
        {
            return ["normal", "with_return"];
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

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            if (placement == "with_return")
                fieldInfo.AddNodes(new Point(16, 0), new Point(32, 0));
        }
    }
}