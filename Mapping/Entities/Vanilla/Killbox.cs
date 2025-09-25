using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Killbox : CSEntityData
    {
        public override string EntityName => "killbox";

        public override List<string> PlacementNames()
        {
            return ["killbox"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"width", 8}
            };
        }

        public override Rectangle Rectangle(RoomData room, Entity entity)
        {
            return new Rectangle(entity.x, entity.y, entity.width, 32, Color(room, entity));
        }

        public override string Color(RoomData room, Entity entity)
        {
            return EdelweissUtils.GetColor(0.8f, 0.4f, 0.4f, 0.8f);
        }
    }
}