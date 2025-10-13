using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class BadelineBossFallingBlock : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "finalBossFallingBlock";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            return [new Tiles("G", true, entity.x, entity.y, entity.width / 8, entity.height / 8)];
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddResizability();
        }
    }
}