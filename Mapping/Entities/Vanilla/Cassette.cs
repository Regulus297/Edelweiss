using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Cassette : CSEntityData
    {
        public override string EntityName => "cassette";

        public override List<string> PlacementNames()
        {
            return ["cassette"];
        }

        public override int Depth(RoomData room, Entity entity) => -10000000;
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override string Texture(RoomData room, Entity entity) => "collectables/cassette/idle00";
        public override List<int> NodeLimits(RoomData room, Entity entity) => [2, 2];
    }
}