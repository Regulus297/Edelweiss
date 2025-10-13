using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class TempleCrackedBlock : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "templeCrackedBlock";

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddResizability(16, 16)
                .AddField("persistent", false);
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            return [new NinePatch("objects/temple/breakBlock00", entity.x, entity.y, entity.width, entity.height)];
        }

        public override List<string> PlacementNames()
        {
            return ["default"];
        }
    }
}