using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class BadelineBossFallingBlock : CSEntityData
    {
        public override string EntityName => "finalBossFallingBlock";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"width", 8},
                {"height", 8}
            };
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            return [new Tiles("G", true, entity.x, entity.y, entity.width / 8, entity.height / 8)];
        }
    }
}