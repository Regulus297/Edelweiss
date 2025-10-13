using System;
using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class FixedBridge : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "bridgeFixed";

        public override List<string> PlacementNames()
        {
            return ["bridge_fixed"];
        }
        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            int x = 0;
            List<Drawable> sprites = [];
            while (x < entity.width)
            {
                Sprite sprite = new Sprite("scenery/bridge_fixed", entity);
                sprite.justificationX = 0;
                sprite.justificationY = 0;
                sprite.x += x;
                sprite.y -= 8;

                x += sprite.data.width;
                sprites.Add(sprite);
            }
            return sprites;
        }

        public override List<Rectangle> Selection(RoomData room, Entity entity)
        {
            Sprite sprite = new Sprite("scenery/bridge_fixed", entity);
            int width = ((entity.width / sprite.data.width) + 1) * sprite.data.width;
            return [new Rectangle(entity.x, entity.y, width, sprite.data.height)];
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddResizability(32, null);
        }
    }
}