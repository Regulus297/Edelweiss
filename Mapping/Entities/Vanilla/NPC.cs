using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class NPC : CSEntityData
    {
        public override string EntityName => "everest/npc";


        public override List<string> PlacementNames()
        {
            return ["npc"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"sprite", "player/idle"},
                {"spriteRate", 1},
                {"dialogId", ""},
                {"onlyOnce", true},
                {"endLevel", false},
                {"flipX", false},
                {"flipY", false},
                {"approachWhenTalking", false},
                {"approachDistance", 16},
                {"indicatorOffsetX", 0},
                {"indicatorOffsetY", 0}
            };
        }

        public override List<string> Mods()
        {
            return ["Everest"];
        }

        public override List<float> Justification(RoomData room, Entity entity) => [0.5f, 1.0f];
        public override int Depth(RoomData room, Entity entity) => 100;
        public override string Texture(RoomData room, Entity entity) => $"characters/{entity["sprite"]}00";
        public override List<float> Scale(RoomData room, Entity entity)
        {
            return [(bool)entity["flipX"] ? -1 : 1, (bool)entity["flipY"] ? -1 : 1];
        }

        public override bool Flip(RoomData room, Entity entity, bool horizontal, bool vertical)
        {
            CycleBoolean(entity, "flipX", horizontal ? 1 : 0);
            CycleBoolean(entity, "flipY", vertical ? 1: 0);
            return true;
        }
    }
}