using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Trapdoor : CSEntityData
    {
        private static readonly string Col = EdelweissUtils.GetColor(22, 27, 48);
        public override string EntityName => "trapdoor";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            return [new Rect(entity.x, entity.y + 6, 24, 4, Col)];
        }
    }
}