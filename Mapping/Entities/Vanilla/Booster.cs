using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Booster : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "booster";

        public override List<string> PlacementNames()
        {
            return ["green", "red"];
        }

        public override int Depth(RoomData room, Entity entity) => -8500;
        public override string Texture(RoomData room, Entity entity)
        {
            bool red = (bool)entity.data["red"];
            return red ? "objects/booster/boosterRed00" : "objects/booster/booster00";
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("red", placement == "red")
                .AddField("ch9_hub_booster", false)
                .SetCyclableField("red");
        }
    }
}