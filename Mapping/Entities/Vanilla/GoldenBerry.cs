using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class GoldenBerry : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "goldenBerry";

        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, -1];
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Fan;
        public override List<string> PlacementNames()
        {
            return ["golden", "golden_winged"];
        }

        public override string Texture(RoomData room, Entity entity)
        {
            bool winged = entity.Get<bool>("winged");
            bool hasNodes = entity.nodes.Count > 0;

            string path = hasNodes ? "ghostgoldberry" : "goldberry";
            string sprite = winged ? "wings01" : "idle00";

            return $"collectables/{path}/{sprite}";
        }

        public override string NodeTexture(RoomData room, Entity entity, int nodeIndex)
        {
            return "collectables/goldberry/seed00";
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("winged", placement == "golden_winged")
                .AddField("moon", false)
                .SetCyclableField("winged");
        }
    }
}