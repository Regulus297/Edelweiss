using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class BounceBlock : CSEntityData
    {
        public override string EntityName => "bounceBlock";

        public override List<string> PlacementNames()
        {
            return ["fire", "ice"];
        }

        public override int Depth(RoomData room, Entity entity) => 8990;

        public override List<int> SizeBounds(RoomData room, Entity entity) => [16, 16, int.MaxValue, int.MaxValue];
        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            bool ice = entity.Get<bool>("notCoreMode");
            string blockTexture = ice ? "objects/BumpBlockNew/ice00" : "objects/BumpBlockNew/fire00";
            string crystalTexture = ice ? "objects/BumpBlockNew/ice_center00" : "objects/BumpBlockNew/fire_center00";

            NinePatch ninePatch = new(blockTexture, entity.x, entity.y, entity.width, entity.height)
            {
                depth = entity.depth
            };

            Sprite crystalSprite = new(crystalTexture, entity);
            crystalSprite.x += entity.width / 2;
            crystalSprite.y += entity.height / 2;

            return [ninePatch, crystalSprite];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"width", 16},
                {"height", 16},
                {"notCoreMode", placement == "ice"}
            };
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            return CycleBoolean(entity, "notCoreMode", amount); 
        }
    }
}