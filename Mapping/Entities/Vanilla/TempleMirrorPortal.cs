using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class TempleMirrorPortal : CSEntityData
    {
        public override string EntityName => "templeMirrorPortal";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override int Depth(RoomData room, Entity entity) => -1999;

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Sprite frame = new Sprite("objects/temple/portal/portalframe", entity);
            Sprite curtain = new Sprite("objects/temple/portal/portalcurtain00", entity);
            Sprite leftTorch = new Sprite("objects/temple/portal/portaltorch00", entity)
            {
                justificationY = 0.75f
            };
            Sprite rightTorch = new Sprite("objects/temple/portal/portaltorch00", entity)
            {
                justificationY = 0.75f
            };

            leftTorch.x -= 90;
            rightTorch.x += 90;

            return [frame, curtain, leftTorch, rightTorch];
        }
    }
}