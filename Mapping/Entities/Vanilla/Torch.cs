using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Torch : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "torch";

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("startLit", true)
                .SetCyclableField("startLit");
        }

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override string Texture(RoomData room, Entity entity) => entity.Get("startLit", true) ? "objects/temple/litTorch03" : "objects/temple/torch00";
    }
}