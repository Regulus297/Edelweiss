using System;
using System.Collections.Generic;

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
            int index = (int)entity["index"];
            return $"scenery/cliffside/flag{index:00}";
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            int index = (int)entity["index"];
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
    }
}