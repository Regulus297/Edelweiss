using System.Collections.Generic;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class SeekerBarrier : CSEntityData
    {
        public override string EntityName => "seekerBarrier";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"width", 8},
                {"height", 8}
            };
        }

        public override string Color(RoomData room, Entity entity) => EdelweissUtils.GetColor(0.25f, 0.25f, 0.25f, 0.8f);
    }
}