using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class RidgeGate : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "ridgeGate";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, 1];
        public override List<float> Justification(RoomData room, Entity entity) => [0, 0];
        public override string Texture(RoomData room, Entity entity) => entity.Get("texture", "objects/ridgeGate");
        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("texture", "objects/ridgeGate")
                .AddField("strawberries", "")
                .AddField("keys", "");
        }
    }
}