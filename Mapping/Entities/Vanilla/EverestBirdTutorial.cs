using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class EverestBirdTutorial : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "everest/customBirdTutorial";

        public override List<string> PlacementNames()
        {
            return ["bird"];
        }

        public override int Depth(RoomData room, Entity entity) => -10000000;
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override List<float> Justification(RoomData room, Entity entity) => [0.5f, 1.0f];
        public override string Texture(RoomData room, Entity entity) => "characters/bird/crow00";
        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, -1];

        public override List<string> Mods()
        {
            return ["Everest"];
        }

        public override List<float> Scale(RoomData room, Entity entity)
        {
            return [entity.Get<bool>("faceLeft") ? -1 : 1, 1];
        }


        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("faceLeft", true)
                .AddField("birdId", "")
                .AddField("onlyOnce", false)
                .AddField("caw", true)
                .AddOptionsField("info", "TUTORIAL_DREAMJUMP", "TUTORIAL_CLIMB", "TUTORIAL_HOLD", "TUTORIAL_DASH", "TUTORIAL_DREAMJUMP", "TUTORIAL_CARRY", "hyperjump/tutorial00", "hyperjump/tutorial01")
                .AddField("controls", "DownRight,+,Dash,tinyarrow,Jump")
                .SetHorizontalFlipField("faceLeft");
        }
    }
}