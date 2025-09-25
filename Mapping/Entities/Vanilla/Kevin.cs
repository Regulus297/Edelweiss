using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Kevin : CSEntityData
    {
        public override string EntityName => "crushBlock";

        public override List<string> PlacementNames()
        {
            return ["both", "vertical", "horizontal"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"width", 24},
                {"height", 24},
                {"axes", placement},
                {"chillout", false}
            };
        }

        // TODO: WarnBelowSize

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            int frameIndex = entity["axes"].ToString() switch
            {
                "both" => 03,
                "vertical" => 02,
                "horizontal" => 01,
                _ => 00
            };

            bool chillout = (bool)entity["chillout"];
            bool giant = entity.width >= 48 && entity.height >= 48 && chillout;

            NinePatch frame = new NinePatch($"objects/crushblock/block{frameIndex:00}", entity.x, entity.y, entity.width, entity.height, "border");
            Rectangle rectangle = new Rectangle(entity.x + 2, entity.y + 2, entity.width - 4, entity.height - 4, EdelweissUtils.GetColor(98, 34, 43));
            Sprite face = new Sprite(giant ? "objects/crushblock/giant_block00" : "objects/crushblock/idle_face", entity);
            face.x += entity.width / 2;
            face.y += entity.height / 2;


            return [rectangle, face, frame];
        }

        public override bool Rotate(RoomData room, Entity entity, int rotation)
        {
            entity["axes"] = entity["axes"].ToString() switch
            {
                "horizontal" => "vertical",
                "vertical" => "horizontal",
                _ => entity["axes"]
            };
            return true;
        }
    }
}