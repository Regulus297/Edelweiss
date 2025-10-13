using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class BirdPath : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "birdPath";
        public override List<string> PlacementNames()
        {
            return ["bird_path"];
        }
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override string Texture(RoomData room, Entity entity) => "characters/bird/flyup00";
        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, -1];

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("only_once", false)
                .AddField("onlyIfLeft", false)
                .AddFloatField("speedMult", 1)
                .AddField("angleFix", false)
                .AddFloatField("angleFixMaxRotation", 60);
        }
    }
}