using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Strawberry : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "strawberry";

        public override List<string> PlacementNames()
        {
            return ["normal", "winged", "moon"];
        }

        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, -1];
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Fan;

        public override string Texture(RoomData room, Entity entity)
        {
            bool moon = entity.Get<bool>("moon");
            bool winged = entity.Get<bool>("winged");
            bool nodes = entity.nodes.Count > 0;

            return moon ? ((winged || nodes) ? "collectables/moonBerry/ghost00" : "collectables/moonBerry/normal00") : (winged ? (nodes ? "collectables/ghostberry/wings01" : "collectables/strawberry/wings01") : (nodes ? "collectables/ghostberry/idle00" : "collectables/strawberry/normal00"));
        }

        public override string NodeTexture(RoomData room, Entity entity, int nodeIndex)
        {
            return "collectables/strawberry/seed00";
        }

        public override int Depth(RoomData room, Entity entity) => -100;

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("winged", placement == "winged")
                .AddField("moon", placement == "moon")
                .AddField("checkpointID", -1)
                .AddField("order", -1);
        }
    }
}