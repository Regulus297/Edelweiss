using System.Collections.Generic;
using System.Linq;
using Edelweiss.Mapping.Entities.Helpers;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class BirdNPC : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "bird";

        private static Dictionary<string, int> modeFacingScale = new Dictionary<string, int>()
        {
            {"ClimbingTutorial", -1},
            {"DashingTutorial", 1},
            {"DreamJumpTutorial", 1},
            {"SuperWallJumpTutorial", -1},
            {"HyperJumpTutorial", -1},
            {"MoveToNodes", -1},
            {"WaitForLightningOff", -1},
            {"FlyAway", -1},
            {"Sleeping", 1},
            {"None", -1}
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

        public override List<float> Scale(RoomData room, Entity entity)
        {
            return [modeFacingScale.GetValueOrDefault(entity.Get<string>("mode"), 1), 1];
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddOptionsField("mode", "Sleeping", [.. modeFacingScale.Keys])
                .AddField("onlyOnce", false)
                .AddField("onlyIfPlayerLeft", false)
                .SetCyclableField("mode");
        }
    }
}