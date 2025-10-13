using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Cloud : CSEntityData, IFieldInfoEntity
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

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("fragile", placement == "fragile")
                .AddField("small", false)
                .SetCyclableField("fragile");
        }
    }
}