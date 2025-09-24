using System;
using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class GlassBlock : CSEntityData
    {
        public override string EntityName => "glassBlock";

        public override string FillColor(RoomData room, Entity entity) => "#99ffffff";
        public override string BorderColor(RoomData room, Entity entity) => "#ccffffff";

        public override List<string> PlacementNames()
        {
            return ["glass_block"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"sinks", false},
                {"width", 8},
                {"height", 8}
            };
        }
    }
}