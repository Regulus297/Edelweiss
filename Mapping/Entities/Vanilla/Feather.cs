using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Loenn;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Feather : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "infiniteStar";

        public override List<string> PlacementNames()
        {
            return ["normal"];
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

        public override List<Rectangle> Selection(RoomData room, Entity entity)
        {
            if (entity.Get<bool>("shielded"))
                return [new Rectangle(entity.x - 12, entity.y - 12, 24, 24)];
            return [new Sprite("objects/flyFeather/idle00", entity).Bounds()];
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("shielded", false)
                .AddField("singleUse", false)
                .SetCyclableField("shielded");
        }
    }
}