using System.Collections.Generic;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class IceBlock : CSEntityData
    {
        public override string EntityName => "iceBlock";

        public override List<string> PlacementNames()
        {
            return ["ice_block"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"width", 8},
                {"height", 8}
            };
        }

        public override string FillColor(RoomData room, Entity entity) => EdelweissUtils.GetColor(76, 168, 214, 102);
        public override string BorderColor(RoomData room, Entity entity) => EdelweissUtils.GetColor(108, 214, 235);
        public override int Depth(RoomData room, Entity entity) => -8500;
    }
}