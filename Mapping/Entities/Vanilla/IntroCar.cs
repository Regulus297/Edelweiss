using System;
using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class IntroCar : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "introCar";

        public override List<string> PlacementNames()
        {
            return ["intro_car"];
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            bool road = entity.Get<bool>("hasRoadAndBarriers");

            Sprite body = new Sprite("scenery/car/body", entity);
            body.justificationY = 1.0f;
            body.depth = 1;

            Sprite wheel = new Sprite("scenery/car/wheels", entity);
            wheel.justificationY = 1.0f;
            body.depth = 3;

            if (!road)
                return [body, wheel];

            List<Sprite> pavements = [];
            Random random = new Random(entity.x + entity.y * room.x + room.y);

            int columns = (entity.x - 48) / 8;
            for (int i = 0; i < columns; i++)
            {
                int choice = i >= columns - 2 ? i == columns - 2 ? 2 : 3 : random.Next(0, 2);
                Sprite pavement = new Sprite("scenery/car/pavement", entity)
                {
                    depth = -10001,
                    justificationX = 0,
                    justificationY = 0,
                    sourceX = choice * 8,
                    sourceWidth = 8,
                    sourceHeight = 8
                };
                pavement.x += i * 8 - entity.x;
                pavements.Add(pavement);
            }

            Sprite barrier1 = new Sprite("scenery/car/barrier", entity)
            {
                justificationX = 0,
                justificationY = 1,
                depth = -10
            };
            barrier1.x += 32;

            Sprite barrier2 = new Sprite("scenery/car/barrier", entity)
            {
                justificationX = 0,
                justificationY = 1,
                depth = 5,
                color = EdelweissUtils.GetColor(169, 169, 169, 255)
            };
            barrier2.x += 41;

            return [body, wheel, .. pavements, barrier1, barrier2];
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("hasRoadAndBarriers", false)
                .SetCyclableField("hasRoadAndBarriers");
        }
    }
}