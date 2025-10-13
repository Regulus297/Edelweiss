using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class BadelineChaser : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "darkChaser";

        public override List<string> PlacementNames()
        {
            return ["dark_chaser"];
        }

        public override List<float> Justification(RoomData room, Entity entity) => [0.5f, 1.0f];
        public override string Texture(RoomData room, Entity entity) => "characters/badeline/sleep00";

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("canChangeMusic", true);
        }
    }
}