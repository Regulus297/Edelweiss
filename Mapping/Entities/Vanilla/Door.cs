using System.Collections.Generic;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Door : CSEntityData
    {
        public override string EntityName => "door";
        public override int Depth(RoomData room, Entity entity) => 8998;
        public override List<float> Justification(RoomData room, Entity entity) => [0.5f, 1.0f];

        public override List<string> PlacementNames()
        {
            return ["wood", "metal"];
        }

        public override string Texture(RoomData room, Entity entity)
        {
            return entity.Get("type", "wood") == "wood" ? "objects/door/door00" : "objects/door/metaldoor00";
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"type", placement}
            };
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            entity["type"] = PlacementNames().Cycle(entity.Get("type", "wood"), amount);
            return true;
        }

        public override JObject FieldInformation(string fieldName)
        {
            return new JObject()
            {
                {"items", new JArray() {
                    "wood", "metal"
                }}
            };
        }
    }
}