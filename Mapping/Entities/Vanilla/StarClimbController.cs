using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class StarClimbController : CSEntityData
    {
        public override string EntityName => "starClimbController";

        public override List<string> PlacementNames()
        {
            return [];
        }
        public override string Texture(RoomData room, Entity entity) => "@Internal@/northern_lights";
    }

    internal class EverestStarClimbController : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "everest/starClimbGraphicsController";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }
        public override string Texture(RoomData room, Entity entity) => "@Internal@/northern_lights";
        
        public override List<string> Mods() => ["Everest"];

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("fgColor", "#A3FFFF")
                .AddField("bgColor", "#293E4B");
        }
    }
}