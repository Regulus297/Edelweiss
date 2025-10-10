using System;
using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    // My puffer has the fucking white lines
    // This is the best and most important feature in the editor
    internal class Puffer : CSEntityData
    {
        public override string EntityName => "eyebomb";

        private static List<(float, float, float, float)> linePositions = [];

        public override void OnRegister()
        {
            base.OnRegister();

            // Populate lines here because I don't want to call SinCos every time the entity is drawn
            for (int i = 0; i < 28; i++)
            {
                float angle = MathF.PI * i / 27f;
                (float oy, float ox) = MathF.SinCos(angle);
                float x1 = 32f * ox;
                float y1 = 32f * oy;

                float length = (i == 0 || i == 27) ? 10f : 3f;
                float x2 = x1 - ox * length;
                float y2 = y1 - oy * length;
                linePositions.Add((x1, y1, x2, y2));
            }
        }

        public override List<string> PlacementNames()
        {
            return ["left", "right"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"right", placement == "right"}
            };
        }

        public override List<float> Scale(RoomData room, Entity entity)
        {
            return [entity.Get<bool>("right") ? 1 : -1, 1];
        }

        public override bool Flip(RoomData room, Entity entity, bool horizontal, bool vertical)
        {
            CycleBoolean(entity, "right", horizontal ? 1 : 0);
            return horizontal;
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            List<Drawable> sprites = [];

            sprites.Add(new Sprite("objects/puffer/idle00", entity));
            foreach ((float x1, float y1, float x2, float y2) in linePositions)
            {
                sprites.Add(new Line(x1 + entity.x, y1 + entity.y, x2 + entity.x, y2 + entity.y));
            }

            return sprites;
        }
    }
}