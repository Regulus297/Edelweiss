using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class NegaBlock : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "negaBlock";

        public override string Color(RoomData room, Entity entity) => "#ff0000";

        public override List<string> PlacementNames()
        {
            return ["nega_block"];
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddResizability();
        }
    }
}