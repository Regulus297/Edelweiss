using System;
using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class TempleEye : CSEntityData
    {
        public override string EntityName => "templeEye";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        private bool IsBackground(RoomData room, Entity entity)
        {
            int tileX = entity.x / 8;
            int tileY = entity.y / 8;
            return room.fgTileData.GetTile(tileX, tileY) == ' ';
        }

        public override int Depth(RoomData room, Entity entity) => IsBackground(room, entity) ? 8990 : -10001;
        public override void Draw(JArray shapes, RoomData room, Entity entity)
        {
            int dx = -entity.x;
            int dy = -entity.y;
            float angle = MathF.Atan2(dy, dx);

            float x = entity.x + MathF.Cos(angle) * 2;
            float y = entity.y + MathF.Sin(angle) * 2;

            string layer = IsBackground(room, entity) ? "bg" : "fg";

            Sprite eye = new Sprite($"scenery/temple/eye/{layer}_eye", entity);
            Sprite lid = new Sprite($"scenery/temple/eye/{layer}_lid00", entity);
            Sprite pupil = new Sprite($"scenery/temple/eye/{layer}_pupil", new Point((int)x, (int)y))
            {
                depth = entity.depth
            };

            using SpriteDestination dest = new SpriteDestination(shapes, entity.x, entity.y);
            eye.Draw();
            lid.Draw();
            pupil.Draw();
        }

        public override List<Rectangle> Selection(RoomData room, Entity entity)
        {
            return [new Sprite("scenery/temple/eye/bg_eye", entity).Bounds()];
        }
    }
}