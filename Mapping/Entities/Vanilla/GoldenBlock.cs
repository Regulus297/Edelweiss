using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class GoldenBlock : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "goldenBlock";

        public override List<string> PlacementNames()
        {
            return ["golden_block"];
        }

        public override int Depth(RoomData room, Entity entity) => -10000;

        // TODO: Add warnBelowSize

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            NinePatch ninePatch = new NinePatch("objects/goldblock", entity.x, entity.y, entity.width, entity.height);
            Sprite middle = new Sprite("collectables/goldberry/idle00", entity);
            middle.x += entity.width / 2;
            middle.y += entity.height / 2;
            return [ninePatch, middle];
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddResizability(16, 16);
        }
    }
}