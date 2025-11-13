using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class TheoCrystalPedestal : CSEntityData
    {
        public override string EntityName => "theoCrystalPedestal";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override int Depth(RoomData room, Entity entity) => 8998;
        public override List<float> Justification(RoomData room, Entity entity) => [0.5f, 1.0f];
        public override string Texture(RoomData room, Entity entity) => "characters/theoCrystal/pedestal";
    }
}