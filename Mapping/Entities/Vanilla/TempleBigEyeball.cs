using System;
using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class TempleBigEyeball : CSEntityData
    {
        public override string EntityName => "templeBigEyeball";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override void Draw(JArray shapes, RoomData room, Entity entity)
        {
            int dx = -entity.x;
            int dy = -entity.y;
            float angle = MathF.Atan2(dy, dx);
            float ox = MathF.Cos(angle) * 10;
            float oy = MathF.Sin(angle) * 10;
            if (MathF.Abs(dx) < MathF.Abs(ox))
                ox = dx;
            if (MathF.Abs(dy) < MathF.Abs(oy))
                oy = dy;

            Sprite body = new Sprite("danger/templeeye/body00", entity);
            Sprite pupil = new Sprite("danger/templeeye/pupil", entity);
            pupil.x += (int)ox;
            pupil.y += (int)oy;

            using SpriteDestination destination = new SpriteDestination(shapes, entity.x, entity.y);
            body.Draw();
            pupil.Draw();
        }

        public override List<Rectangle> Selection(RoomData room, Entity entity)
        {
            return [new Sprite("danger/templeeye/body00", entity).Bounds()];
        }
    }
}