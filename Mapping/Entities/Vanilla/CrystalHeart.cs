using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class CrystalHeart : CSEntityData
    {
        public override string EntityName => "blackGem";

        public override List<string> PlacementNames()
        {
            return ["crystal_heart"];
        }

        public override int Depth(RoomData room, Entity entity) => -2000000;
        public override string Texture(RoomData room, Entity entity) => "collectables/heartGem/0/00";
        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"fake", false},
                {"removeCameraTriggers", false},
                {"fakeHeartDialog", "CH9_FAKE_HEART"},
                {"keepGoingDialog", "CH9_KEEP_GOING"}
            };
        }
    }
}