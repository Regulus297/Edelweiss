using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Cloud : CSEntityData
    {
        public override string EntityName => "cloud";

        public override List<string> PlacementNames()
        {
            return ["normal", "fragile"];
        }

        private string GetTexture(Entity entity)
        {
            bool fragile = entity.Get<bool>("fragile");
            return fragile ? "objects/clouds/fragile00" : "objects/clouds/cloud00";
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Sprite sprite = new(GetTexture(entity), entity);
            bool small = entity.Get<bool>("small");
            float scale = small ? 29f / 35 : 1.0f;
            sprite.scaleX = scale;
            return [sprite];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"fragile", placement == "fragile"},
                {"small", false}
            };
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            return CycleBoolean(entity, "fragile", amount);
        }
    }
}