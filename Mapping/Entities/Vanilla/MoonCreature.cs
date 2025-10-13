using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class MoonCreature : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "moonCreature";

        public override List<string> PlacementNames()
        {
            return ["moon_creature"];
        }

        public override int Depth(RoomData room, Entity entity) => -1000000;
        public override string Texture(RoomData room, Entity entity) => "scenery/moon_creatures/tiny05";

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("number", 1);
        }
    }
}