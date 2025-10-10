using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class CliffsideFlag : CSEntityData
    {
        public override string EntityName => "cliffside_flag";

        public override List<string> PlacementNames()
        {
            return ["cliffside_flag"];
        }

        public override int Depth(RoomData room, Entity entity) => 8999;
        public override List<float> Justification(RoomData room, Entity entity) => [0f, 0f];
        public override string Texture(RoomData room, Entity entity)
        {
            int index = entity.Get<int>("index");
            return $"scenery/cliffside/flag{index:00}";
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            int index = entity.Get<int>("index");
            index += amount;
            index %= 11;
            entity["index"] = index;
            return true;
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"index", 0}
            };
        }

        public override JObject FieldInformation(string fieldName)
        {
            if (fieldName != "index")
                return null;
            return new JObject()
            {
                {"fieldType", "integer"},
                {"items", new JArray() {
                    0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10
                }}
            };
        }
    }
}