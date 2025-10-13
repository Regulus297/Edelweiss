using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Killbox : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "killbox";

        public override List<string> PlacementNames()
        {
            return ["killbox"];
        }

        public override Rectangle Rectangle(RoomData room, Entity entity)
        {
            return new Rectangle(entity.x, entity.y, entity.width, 32);
        }

        public override string Color(RoomData room, Entity entity)
        {
            return EdelweissUtils.GetColor(0.8f, 0.4f, 0.4f, 0.8f);
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddResizability(8, null);
        }
    }
}