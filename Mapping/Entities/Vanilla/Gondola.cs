using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Gondola : CSEntityData
    {
        public override string EntityName => "gondola";

        public override List<string> PlacementNames()
        {
            return ["gondola"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"active", true}
            };
        }

        public override int Depth(RoomData room, Entity entity) => -10500;
        public override List<int> NodeLimits(RoomData room, Entity entity) => [1, 1];
        public override Visibility NodeVisibility(Entity entity) => Visibility.Always;

        private Point GetGondolaPosition(Entity entity)
        {
            bool active = entity.Get("active", true);
            return active ? new Point(entity.x, entity.y) : entity.GetNode(0);
        }

        private List<Drawable> AddGondolaMainSprites(Entity entity)
        {
            bool active = entity.Get("active", true);
            Point pos = GetGondolaPosition(entity);

            Sprite front = new Sprite("objects/gondola/front", pos);
            front.y += -64;
            front.justificationY = 0f;

            Sprite top = new Sprite("objects/gondola/top", pos);
            top.y += -64;
            top.justificationY = 0f;

            Sprite lever = new Sprite("objects/gondola/lever01", pos);
            lever.y += -64;
            lever.justificationY = 0f;

            Sprite back = new Sprite("objects/gondola/back", pos);
            back.y += -64;
            back.justificationY = 0f;
            back.depth = 9000;

            return active ? [front, top, lever, back] : [front, top, back];
        }

        private Sprite GetLeftSprite(Entity entity)
        {
            Sprite left = new Sprite("objects/gondola/cliffsideLeft", entity);
            left.x += -124;
            left.justificationX = 0f;
            left.justificationY = 1f;
            left.depth = 8998;
            return left;
        }

        private Sprite GetRightSprite(Entity entity)
        {
            Point node = entity.GetNode(0);
            Sprite right = new Sprite("objects/gondola/cliffsideRight", node);
            right.x += 144;
            right.y += -104;
            right.justificationX = 0f;
            right.scaleX = -1;
            right.depth = 8998;
            return right;
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            bool active = entity.Get("active", true);
            Point pos = GetGondolaPosition(entity);
            Sprite left = GetLeftSprite(entity);
            Sprite right = GetRightSprite(entity);

            Sprite top = new Sprite("objects/gondola/top", pos);
            top.y += -64;
            top.justificationY = 0f;

            Point wireLeftPos = new Point(left.x + 40, left.y - 12);
            Point wireRightPos = new Point(right.x - 40, right.y - 4);

            Point topPos = new Point(pos.X, pos.Y - 64 + top.data.height);

            Line leftWire = new Line(wireLeftPos, topPos, "#000000", 1);
            Line rightWire = new Line(topPos, wireRightPos, "#000000", 1);

            leftWire.depth = rightWire.depth = 8999;

            return active ? [leftWire, rightWire, .. AddGondolaMainSprites(entity), left] : [leftWire, rightWire, left];
        }

        // TODO: selection
        public override List<Rectangle> Selection(RoomData room, Entity entity)
        {
            Point pos = GetGondolaPosition(entity);

            Sprite front = new Sprite("objects/gondola/front", pos)
            {
                justificationY = 0f
            };
            front.y += -64;

            Sprite top = new Sprite("objects/gondola/top", pos)
            {
                justificationY = 0f
            };
            top.y += -64;

            Rectangle gondolaRect = front.Bounds().Combine(top.Bounds());

            if(entity.Get("active", true))
            {
                Sprite right = GetRightSprite(entity);
                return [gondolaRect, right.Bounds()];
            }

            Sprite left = GetLeftSprite(entity);
            return [gondolaRect, left.Bounds()];
        }

        public override List<Drawable> NodeSprite(RoomData room, Entity entity, int nodeIndex)
        {
            Sprite right = GetRightSprite(entity);
            bool active = entity.Get("active", true);
            return active ? [right] : [right, .. AddGondolaMainSprites(entity)];
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            entity["active"] = !entity.Get("active", true);
            return true;
        }
    }
}