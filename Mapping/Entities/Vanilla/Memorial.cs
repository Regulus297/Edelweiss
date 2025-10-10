using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Memorial : CSEntityData
    {
        public override string EntityName => "everest/memorial";

        public override List<string> PlacementNames()
        {
            return ["memorial"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"dialog", "MEMORIAL"},
                {"sprite", "scenery/memorial/memorial"},
                {"spacing", 16}
            };
        }

        public override string Texture(RoomData room, Entity entity)
        {
            return entity.Get("sprite", "scenery/memorial/memorial");
        }

        public override List<string> Mods()
        {
            return ["Everest"];
        }

        public override int Depth(RoomData room, Entity entity) => 100;
        public override List<float> Justification(RoomData room, Entity entity) => [0.5f, 1.0f];
    }
}