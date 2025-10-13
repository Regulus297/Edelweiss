using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;
using Edelweiss.Mapping.Tools;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class CrumbleBlock : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "crumbleBlock";

        public override List<string> PlacementNames()
        {
            return ["default", "cliffside"];
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            return [new NinePatch($"objects/crumbleBlock/{entity.Get("texture", "default")}", entity.x, entity.y, entity.width, 8, borderLeft: 0, borderRight: 0, borderTop: 0, borderBottom: 0)];
        }

        public override List<Rectangle> Selection(RoomData room, Entity entity)
        {
            return [new Rectangle(entity.x, entity.y, entity.width, 8)];
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddResizability(8, null)
                .AddOptionsField("texture", placement, "default", "cliffside")
                .SetCyclableField("texture");
        }
    }
}