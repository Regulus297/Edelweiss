using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class BadelineBoost : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "badelineBoost";
        public override int Depth(RoomData room, Entity entity)
        {
            return -1000000;
        }

        public override NodeLineRenderType NodeLineRenderType(Entity entity)
        {
            return Entities.NodeLineRenderType.Line;
        }

        public override string Texture(RoomData room, Entity entity)
        {
            return "objects/badelineboost/idle00";
        }

        public override List<int> NodeLimits(RoomData room, Entity entity)
        {
            return [0, -1];
        }

        public override List<string> PlacementNames()
        {
            return ["boost"];
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("lockCamera", true)
                .AddField("canSkip", false)
                .AddField("finalCh9Boost", false)
                .AddField("finalCh9GoldenBoost", false)
                .AddField("finalCh9Dialog", false);
        }
    }
}