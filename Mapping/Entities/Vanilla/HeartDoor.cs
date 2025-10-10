using System;
using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class HeartDoor : CSEntityData
    {
        public override string EntityName => "heartGemDoor";

        public override List<string> PlacementNames()
        {
            return ["door"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"width", 40},
                {"requires", 0},
                {"startHidden", false}
            };
        }

        // TODO: warnbelowsize

        private int HeartsWidth(int spriteWidth, int hearts) => hearts * (spriteWidth + 4) - 4;

        private int HeartsPossible(int edgeWidth, int spriteWidth, int width, int required)
        {
            int rowWidth = width - 2 * edgeWidth;
            for (int i = 0; i <= required; i++)
            {
                if (HeartsWidth(spriteWidth, i) > rowWidth)
                {
                    return i - 1;
                }
            }
            return required;
        }

        public override List<int> NodeLimits(RoomData room, Entity entity) => [0, 1];

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            int x = entity.x;
            int y = 0;

            Rect bg = new Rect(x, y, entity.width, room.height, EdelweissUtils.GetColor(47, 187, 255));
            List<Drawable> sprites = [];

            for (int i = 0; i < room.height; i += 8)
            {
                Sprite edgeLeft = new Sprite("objects/heartdoor/edge", entity);
                edgeLeft.y = i;
                edgeLeft.scaleX = -1;
                edgeLeft.justificationY = 0;
                edgeLeft.justificationX = 1;
                sprites.Add(edgeLeft);

                Sprite edgeRight = new Sprite("objects/heartdoor/edge", entity);
                edgeRight.x += entity.width;
                edgeRight.y = i;
                edgeRight.justificationY = 0;
                edgeRight.justificationX = 1;
                sprites.Add(edgeRight);
            }

            int requires = entity.Get<int>("requires");
            if (requires > 0)
            {
                int fits = HeartsPossible(5, 10, entity.width, requires);
                if (fits > 0)
                {
                    int rows = 1 + requires / fits;
                    for (int i = 1; i <= rows; i++)
                    {
                        int displayed = HeartsPossible(5, 10, entity.width, requires);
                        int drawWidth = HeartsWidth(10, displayed);

                        int startX = x + (int)Math.Round((entity.width - drawWidth) / 2f) + 5 - 2;
                        int startY = y + room.height / 2 - (int)Math.Round(rows / 2f * (10 + 4)) - 4 - 2;

                        for (int j = 1; j <= displayed; j++)
                        {
                            int drawX = startX + (j - 1) * (10 + 4) - 4;
                            int drawY = startY + i * (10 + 4) - 4;

                            Sprite heart = new Sprite("objects/heartdoor/icon00");
                            heart.x = drawX;
                            heart.y = drawY;
                            heart.justificationX = heart.justificationY = 0;
                            sprites.Add(heart);
                        }
                        requires -= displayed;
                    }
                }
            }


            return [bg, .. sprites];
        }

        // TODO: drawselected
        public override List<Rectangle> Selection(RoomData room, Entity entity)
        {
            if (entity.nodes.Count == 0)
            {
                return [new Rectangle(entity.x, 0, entity.width, room.height)];
            }
            Point node = entity.GetNode(0);
            return [new Rectangle(entity.x, 0, entity.width, room.height), new Rectangle(node.X - 8, node.Y, entity.width + 16, 8)];
        }
    }
}