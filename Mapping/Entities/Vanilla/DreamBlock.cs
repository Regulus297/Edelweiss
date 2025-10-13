using System;
using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class DreamBlock : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "dreamBlock";

        public override string FillColor(RoomData room, Entity entity) => "#000000";
        public override string BorderColor(RoomData room, Entity entity) => "#ffffff";
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, 1];

        public override List<string> PlacementNames()
        {
            return ["dream_block"];
        }

        public override int Depth(RoomData room, Entity entity)
        {
            return entity.Get<bool>("below") ? 5000 : -11000;
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("fastMoving", false)
                .AddField("below", false)
                .AddField("oneUse", false)
                .AddResizability()
                .SetCyclableField("below");
        }
    }
}