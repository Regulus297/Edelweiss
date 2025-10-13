using System;
using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class SwapBlock : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "swapBlock";

        public override List<int> NodeLimits(RoomData room, Entity entity) => [1, 1];

        public override List<string> PlacementNames()
        {
            return ["Normal", "Moon"];
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            List<Drawable> sprites = [];
            AddTrailSprites(sprites, entity);
            AddBlockSprites(sprites, entity, -1);
            return sprites;
        }

        public override List<Drawable> NodeSprite(RoomData room, Entity entity, int nodeIndex)
        {
            List<Drawable> sprites = [];
            AddBlockSprites(sprites, entity, nodeIndex);
            return sprites;
        }

        private void AddTrailSprites(List<Drawable> sprites,Entity entity)
        {
            Point node = entity.GetNode(0);
            int drawWidth = int.Abs(entity.x - node.X) + entity.width;
            int drawHeight = int.Abs(entity.y - node.Y) + entity.height;

            int x = int.Min(entity.x, node.X);
            int y = int.Min(entity.y, node.Y);

            bool normal = entity.Get("theme", "Normal").ToLower() == "normal";
            if (normal)
            {
                string pathDirection = x == node.X ? "V" : "H";
                string pathTexture = $"objects/swapblock/path{pathDirection}";
                NinePatch pathNinePatch = new NinePatch(pathTexture, x, y, drawWidth, drawHeight, borderLeft: 0, borderRight: 0, borderTop: 0, borderBottom: 0)
                {
                    depth = 8999
                };
                sprites.Add(pathNinePatch);
            }
            NinePatch ninePatch = new NinePatch(normal ? "objects/swapblock/target" : "objects/swapblock/moon/target", x, y, drawWidth, drawHeight)
            {
                depth = 8999
            };
            sprites.Add(ninePatch);
        }

        private void AddBlockSprites(List<Drawable> sprites, Entity entity, int nodeIndex)
        {
            bool normal = entity.Get("theme", "Normal").ToLower() == "normal";
            int x = nodeIndex >= 0 ? entity.GetNode(nodeIndex).X : entity.x;
            int y = nodeIndex >= 0 ? entity.GetNode(nodeIndex).Y : entity.y;

            NinePatch frame = new NinePatch(normal ? "objects/swapblock/blockRed" : "objects/swapblock/moon/blockRed", x, y, entity.width, entity.height)
            {
                depth = -9999
            };
            if (nodeIndex >= 0)
            {
                frame.color = "#b3ffffff";
            }

            sprites.Add(frame);

            Sprite middle = new Sprite(normal ? "objects/swapblock/midBlockRed00" : "objects/swapblock/moon/midBlockRed00", new Point(x, y));
            middle.x += entity.width / 2;
            middle.y += entity.height / 2;
            middle.depth = -9999;
            sprites.Add(middle);
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddOptionsField("theme", placement, "Normal", "Moon")
                .AddResizability(16, 16)
                .SetCyclableField("theme");
        }
    }
}