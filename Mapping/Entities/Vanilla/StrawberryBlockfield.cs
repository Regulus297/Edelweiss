using System.Collections.Generic;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class StrawberryBlockfield : CSEntityData
    {
        public override string EntityName => "blockField";

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

        public override string FillColor(RoomData room, Entity entity) => EdelweissUtils.GetColor(0.4f, 0.4f, 1.0f, 0.4f);
        public override string BorderColor(RoomData room, Entity entity) => EdelweissUtils.GetColor(0.4f, 0.4f, 1.0f, 1.0f);
    }
}