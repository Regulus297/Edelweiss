using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Payphone : CSEntityData
    {
        public override string EntityName => "payphone";

        public override List<string> PlacementNames()
        {
            return ["payphone"];
        }

        public override List<float> Justification(RoomData room, Entity entity) => [0.5f, 1.0f];
        public override int Depth(RoomData room, Entity entity) => 1;
        public override string Texture(RoomData room, Entity entity) => "scenery/payphone";
    }
}