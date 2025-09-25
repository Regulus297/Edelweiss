using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class EverestBirdTutorial : CSEntityData
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

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"faceLeft", true},
                {"birdId", ""},
                {"onlyOnce", false},
                {"caw", true},
                {"info", "TUTORIAL_DREAMJUMP"},
                {"controls", "DownRight,+,Dash,tinyarrow,Jump"}
            };
        }

        public override List<float> Scale(RoomData room, Entity entity)
        {
            return [(bool)entity["faceLeft"] ? -1 : 1, 1];
        }

        public override bool Flip(RoomData room, Entity entity, bool horizontal, bool vertical)
        {
            if (horizontal)
                entity["faceLeft"] = !(bool)entity["faceLeft"];
            return horizontal;
        }
    }
}