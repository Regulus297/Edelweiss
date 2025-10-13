using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class CrystalHeart : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "blackGem";

        public override List<string> PlacementNames()
        {
            return ["crystal_heart"];
        }

        public override int Depth(RoomData room, Entity entity) => -2000000;
        public override string Texture(RoomData room, Entity entity) => "collectables/heartGem/0/00";

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("fake", false)
                .AddField("removeCameraTriggers", false)
                .AddField("fakeHeartDialog", "CH9_FAKE)HEART")
                .AddField("keepGoingDialog", "CH9_KEEP_GOING");
        }
    }
}