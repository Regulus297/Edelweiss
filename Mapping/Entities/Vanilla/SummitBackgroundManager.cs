using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class SummitBackgroundManager : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "SummitBackgroundManager";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }
        
        public override string Texture(RoomData room, Entity entity) => "@Internal@/summit_background_manager";

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("index", 0)
                .AddField("cutscene", "")
                .AddField("intro_launch", false)
                .AddField("dark", false)
                .AddField("ambience", "");
        }
    }
}