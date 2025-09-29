using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class HangingLamp : CSEntityData
    {
        public override string EntityName => "hanginglamp";

        public override List<string> PlacementNames()
        {
            return ["hanging_lamp"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"height", 16}
            };
        }

        public override int Depth(RoomData room, Entity entity) => 2000;

        // TODO: warnbelowsize, selection
        public override List<Rectangle> Selection(RoomData room, Entity entity)
        {
            return [new Rectangle(entity.x, entity.y, 8, entity.height)];
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            List<Drawable> sprites = [];

            Sprite top = new Sprite("objects/hanginglamp", entity);
            top.justificationX = top.justificationY = 0;
            top.sourceWidth = top.sourceHeight = 8;

            sprites.Add(top);

            // This is when i realised lua loops are inclusive upper bound
            // literally whar
            for (int i = 0; i <= entity.height - 16; i += 8)
            {
                Sprite mid = new Sprite("objects/hanginglamp", entity);
                mid.justificationX = mid.justificationY = 0;
                mid.sourceWidth = mid.sourceHeight = 8;
                mid.sourceY = 8;
                mid.y += i;

                sprites.Add(mid);
            }

            Sprite bottom = new Sprite("objects/hanginglamp", entity);
            bottom.justificationX = bottom.justificationY = 0;
            bottom.sourceWidth = bottom.sourceHeight = 8;
            bottom.sourceY = 16;
            bottom.y += entity.height - 8;
            sprites.Add(bottom);

            return sprites;
        }
    }
}