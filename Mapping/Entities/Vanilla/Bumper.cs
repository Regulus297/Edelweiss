using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Bumper : CSEntityData
    {
        public override string EntityName => "bigSpinner";

        public override List<string> PlacementNames()
        {
            return ["bumper"];
        }

        public override string Texture(RoomData room, Entity entity) => "objects/Bumper/Idle22";
        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, 1];
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
    }
}