using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Tentacles : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "tentacles";

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddOptionsField("fear_distance", "", ("None", ""), ("Close", "close"), ("Medium", "medium"), ("Far", "far"))
                .AddIntField("slide_until", 0);
        }

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override string Texture(RoomData room, Entity entity) => "@Internal@/tentacles";
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override List<int> NodeLimits(RoomData room, Entity entity) => [1, -1];
    }
}