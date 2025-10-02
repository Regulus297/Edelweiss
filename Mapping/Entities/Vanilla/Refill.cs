using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Refill : CSEntityData
    {
        public override string EntityName => "refill";

        public override List<string> PlacementNames()
        {
            return ["single", "double"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"twoDash", placement == "double"},
                {"oneUse", false}
            };
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            string texturePath = (bool)entity["twoDash"] ? "objects/refillTwo/" : "objects/refill/";
            Sprite refill = new Sprite(texturePath + "idle00", entity);
            if ((bool)entity["oneUse"])
            {
                Sprite outline = new Sprite(texturePath + "outline", entity)
                {
                    color = "#ff0000"
                };
                return [refill, outline];
            }
            return [refill];
        }
        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            return CycleBoolean(entity, "twoDash", amount);
        }
    }
}