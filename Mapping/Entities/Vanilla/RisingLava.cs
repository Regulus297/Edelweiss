using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class RisingLava : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "risingLava";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override string Texture(RoomData room, Entity entity) => "@Internal@/rising_lava";

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("intro", false);
        }
    }
}