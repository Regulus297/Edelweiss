using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class TempleMirror : CSEntityData, IFieldInfoEntity
    {
        private static readonly string MirrorColor = EdelweissUtils.GetColor(5, 7, 14);

        public override string EntityName => "templeMirror";

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddResizability(24, 24)
                .AddFloatField("reflectX", 0)
                .AddFloatField("reflectY", 0);
        }

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override int Depth(RoomData room, Entity entity) => 8995;
        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            NinePatch mirror = new NinePatch("scenery/templemirror", entity.x, entity.y, entity.width, entity.height, "border");
            Rect inner = new Rect(entity.x + 2, entity.y + 2, entity.width - 4, entity.height - 4, MirrorColor);
            return [inner, mirror];
        }
    }
}