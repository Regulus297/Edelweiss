using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Fireball : CSEntityData
    {
        public override string EntityName => "fireBall";

        public override List<int> NodeLimits(RoomData room, Entity entity) => [1, -1];
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;

        public override List<string> PlacementNames()
        {
            return ["fireball", "iceball"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"amount", 3},
                {"offset", 0.0},
                {"speed", 1.0},
                {"notCoreMode", placement == "iceball"}
            };
        }

        public override string Texture(RoomData room, Entity entity)
        {
            bool ice = (bool)entity["notCoreMode"];
            return ice ? "objects/fireball/fireball09" : "objects/fireball/fireball01";
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            return CycleBoolean(entity, "notCoreMode", amount);
        }
    }
}