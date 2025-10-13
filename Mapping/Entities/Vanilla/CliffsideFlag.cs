using System;
using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class CliffsideFlag : CSEntityData, IFieldInfoEntity
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

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddOptionsField("index", 0, "integer", 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10)
                .SetCyclableField("index");
        }
    }
}