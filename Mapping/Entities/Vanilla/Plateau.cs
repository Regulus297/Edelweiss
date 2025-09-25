using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Plateau : CSEntityData
    {
        public override string EntityName => "plateau";

        public override List<string> PlacementNames()
        {
            return ["plateau"];
        }

        public override List<float> Justification(RoomData room, Entity entity) => [0, 0];
        public override string Texture(RoomData room, Entity entity) => "scenery/fallplateau";
    }
}