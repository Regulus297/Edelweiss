using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class LavaSandwich : CSEntityData
    {
        public override string EntityName => "sandwichLava";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override string Texture(RoomData room, Entity entity) => "@Internal@/lava_sandwich";
    }
}