using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Tools;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class CrumbleBlock : CSEntityData
    {
        public override string EntityName => "crumbleBlock";

        public override List<string> PlacementNames()
        {
            return ["default", "cliffside"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"width", 8},
                {"texture", placement}
            };
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            return [new NinePatch($"objects/crumbleBlock/{entity["texture"]}", entity.x, entity.y, entity.width, 8, borderLeft: 0, borderRight: 0, borderTop: 0, borderBottom: 0)];
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            entity["texture"] = PlacementNames().Cycle(entity["texture"].ToString(), amount);
            return true;
        }
    }
}