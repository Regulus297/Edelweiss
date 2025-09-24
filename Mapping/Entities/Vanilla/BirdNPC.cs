using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class BirdNPC : CSEntityData
    {
        public override string EntityName => "bird";

        private static Dictionary<string, int> modeFacingScale = new Dictionary<string, int>()
        {
            {"climbingtutorial", -1},
            {"dashingtutorial", 1},
            {"dreamjumptutorial", 1},
            {"superwalljumptutorial", -1},
            {"hyperjumptutorial", -1},
            {"movetonodes", -1},
            {"waitforlightningoff", -1},
            {"flyaway", -1},
            {"sleeping", 1},
            {"none", -1}
        };

        public override List<string> PlacementNames()
        {
            return ["bird"];
        }

        public override int Depth(RoomData room, Entity entity) => -1000000;
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override List<float> Justification(RoomData room, Entity entity) => [0.5f, 1.0f];
        public override string Texture(RoomData room, Entity entity) => "characters/bird/crow00";
        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, -1];

        // TODO: implement scale

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"mode", "Sleeping"},
                {"onlyOnce", false},
                {"onlyIfPlayerLeft", false}
            };
        }

    }
}