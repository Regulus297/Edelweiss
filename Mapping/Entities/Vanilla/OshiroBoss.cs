using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class OshiroBoss : CSEntityData
    {
        public override string EntityName => "friendlyGhost";

        public override List<string> PlacementNames()
        {
            return ["oshiro_boss"];
        }

        public override int Depth(RoomData room, Entity entity) => -12500;
        public override string Texture(RoomData room, Entity entity) => "characters/oshiro/boss13";
    }
}