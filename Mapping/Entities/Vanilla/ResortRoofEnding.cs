using System;
using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class ResortRoofEnding : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "resortRoofEnding";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        private const string StartTexture = "decals/3-resort/roofEdge_d";
        private const string EndTexture = "decals/3-resort/roofEdge";
        private static readonly string[] MidTextures = [
            "decals/3-resort/roofCenter",
            "decals/3-resort/roofCenter_b",
            "decals/3-resort/roofCenter_c",
            "decals/3-resort/roofCenter_d"
        ];
        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Random random = new Random(entity.x * entity.y + room.x * room.y);

            List<Drawable> sprites = [];
            Sprite start = new Sprite(StartTexture, entity);
            start.x += 8;
            start.y += 4;
            sprites.Add(start);

            int offset = 0;
            while (offset < entity.width)
            {
                Sprite mid = new Sprite(MidTextures[random.Next(MidTextures.Length)], entity);
                mid.x += offset + 8;
                mid.y += 4;
                sprites.Add(mid);
                offset += 16;
            }

            Sprite end = new Sprite(EndTexture, entity);
            end.x += offset + 8;
            end.y += 4;
            sprites.Add(end);

            return sprites;
        }

        public override List<Rectangle> Selection(RoomData room, Entity entity)
        {
            return [new Rectangle(entity.x, entity.y, entity.width, 8)];
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddResizability(8, null);
        }
    }
}