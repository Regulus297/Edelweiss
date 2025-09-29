using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class RidgeGate : CSEntityData
    {
        public override string EntityName => "ridgeGate";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, 1];
        public override List<float> Justification(RoomData room, Entity entity) => [0, 0];
        public override string Texture(RoomData room, Entity entity) => entity["texture"].ToString();
        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"texture", "objects/ridgeGate"},
                {"strawberries", ""},
                {"keys", ""}
            };
        }
    }
}