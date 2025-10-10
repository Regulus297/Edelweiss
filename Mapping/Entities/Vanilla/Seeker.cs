using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Seeker : CSEntityData
    {
        public override string EntityName => "seeker";

        public override int Depth(RoomData room, Entity entity) => -199;

        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override string Texture(RoomData room, Entity entity)
        {
            return "characters/monsters/predator73";
        }

        public override List<int> NodeLimits(RoomData room, Entity entity) => [1, -1];

        public override List<string> PlacementNames()
        {
            return ["default"];
        }
    }
}