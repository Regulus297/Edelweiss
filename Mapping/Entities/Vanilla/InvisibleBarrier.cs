using System.Collections.Generic;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class InvisibleBarrier : CSEntityData
    {
        public override string EntityName => "invisibleBarrier";

        public override List<string> PlacementNames()
        {
            return ["invisible_barrier"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"width", 8},
                {"height", 8}
            };
        }

        public override string FillColor(RoomData room, Entity entity) => EdelweissUtils.GetColor(0.4f, 0.4f, 0.4f, 0.8f);
        public override string BorderColor(RoomData room, Entity entity) => "#00000000";
    }
}