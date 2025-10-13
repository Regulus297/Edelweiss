using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Refill : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "refill";

        public override List<string> PlacementNames()
        {
            return ["single", "double"];
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            string texturePath = entity.Get<bool>("twoDash") ? "objects/refillTwo/" : "objects/refill/";
            Sprite refill = new Sprite(texturePath + "idle00", entity);
            if (entity.Get<bool>("oneUse"))
            {
                Sprite outline = new Sprite(texturePath + "outline", entity)
                {
                    color = "#ff0000"
                };
                return [refill, outline];
            }
            return [refill];
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("twoDash", placement == "double")
                .AddField("oneUse", false)
                .SetCyclableField("twoDash");
        }
    }
}