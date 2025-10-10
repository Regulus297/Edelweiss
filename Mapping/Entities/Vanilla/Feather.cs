using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Loenn;
using Edelweiss.Mapping.Drawables;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Feather : CSEntityData
    {
        public override string EntityName => "infiniteStar";

        public override List<string> PlacementNames()
        {
            return ["normal"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"shielded", false},
                {"singleUse", false}
            };
        }

        public override void Draw(JArray shapes, RoomData room, Entity entity)
        {
            Sprite feather = new Sprite("objects/flyFeather/idle00", entity);
            using (new SpriteDestination(shapes, entity.x, entity.y))
            {
                feather.Draw();
                if (!entity.Get<bool>("shielded"))
                {
                    return;
                }

                SpriteDestination.destination.Add(new JObject()
                {
                    {"type", "circle"},
                    {"x", entity.x - SpriteDestination.offsetX},
                    {"y", entity.y - SpriteDestination.offsetY},
                    {"radius", 12},
                    {"color", "#ffffff"},
                    {"thickness", LoveModule.PEN_THICKNESS}
                });
            }
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            return CycleBoolean(entity, "shielded", amount);
        }

        public override List<Rectangle> Selection(RoomData room, Entity entity)
        {
            if (entity.Get<bool>("shielded"))
                return [new Rectangle(entity.x - 12, entity.y - 12, 24, 24)];
            return [new Sprite("objects/flyFeather/idle00", entity).Bounds()];
        }
    }
}