using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class SeekerStatue : CSEntityData
    {
        public override string EntityName => "seekerStatue";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"hatch", "Distance"}
            };
        }

        public override int Depth(RoomData room, Entity entity) => 8999;
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override string Texture(RoomData room, Entity entity) => "decals/5-temple/statue_e";
        public override string NodeTexture(RoomData room, Entity entity, int nodeIndex) => "characters/monsters/predator73";
        public override JObject FieldInformation(string fieldName)
        {
            return new JObject()
            {
                {"items", new JArray() {
                    "Distance", "PlayerRightOfX"
                }}
            };
        }

        public override List<int> NodeLimits(RoomData room, Entity entity) => [1, -1];
    }
}