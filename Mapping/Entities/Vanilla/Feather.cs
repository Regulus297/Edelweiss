using System.Collections.Generic;
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
                if (!(bool)entity["shielded"])
                {
                    return;
                }

                SpriteDestination.destination.Add(new JObject()
                {
                    {"type", "circle"},
                    {"x", (int)entity.x - SpriteDestination.offsetX},
                    {"y", (int)entity.y - SpriteDestination.offsetY},
                    {"radius", 12},
                    {"color", "#ffffff"},
                    {"thickness", LoveModule.PEN_THICKNESS}
                });
            }
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            entity["shielded"] = !(bool)entity["shielded"];
            return true;
        }
    }
}