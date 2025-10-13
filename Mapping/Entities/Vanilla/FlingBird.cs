using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class FlingBird : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "flingBird";

        public override List<string> PlacementNames()
        {
            return ["fling_bird"];
        }

        public override int Depth(RoomData room, Entity entity) => -1;
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, -1];
        public override string Texture(RoomData room, Entity entity) => "characters/bird/Hover04";

        public virtual void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("waiting", false);
        }
    }
}