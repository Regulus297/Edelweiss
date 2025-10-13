using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class BadelineBoss : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "finalBoss";

        public override List<string> PlacementNames()
        {
            return ["boss"];
        }


        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override string Texture(RoomData room, Entity entity) => "characters/badelineBoss/charge00";
        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, -1];

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddOptionsField("patternIndex", 1, "integer", 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15)
                .AddField("startHit", false)
                .AddFloatField("cameraPastY", 120)
                .AddField("cameraLockY", true)
                .AddField("canChangeMusic", true);
        }
    }
}