using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Fireball : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "fireBall";

        public override List<int> NodeLimits(RoomData room, Entity entity) => [1, -1];
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;

        public override List<string> PlacementNames()
        {
            return ["fireball", "iceball"];
        }

        public override string Texture(RoomData room, Entity entity)
        {
            bool ice = entity.Get<bool>("notCoreMode");
            return ice ? "objects/fireball/fireball09" : "objects/fireball/fireball01";
        }
        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("amount", 3)
                .AddField("offset", 0.0f)
                .AddField("speed", 1.0f)
                .AddField("notCoreMode", placement == "iceball")
                .SetCyclableField("notCoreMode");
        }
    }
}