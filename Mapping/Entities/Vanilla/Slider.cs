using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Loenn;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Slider : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "slider";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override void Draw(JArray shapes, RoomData room, Entity entity)
        {
            using SpriteDestination spriteDestination = new SpriteDestination(shapes, entity.x, entity.y);

            SpriteDestination.destination.Add(new JObject()
            {
                {"type", "circle"},
                {"x", entity.x - SpriteDestination.offsetX},
                {"y", entity.y - SpriteDestination.offsetY},
                {"radius", 12},
                {"color", "#ff0000"},
                {"thickness", LoveModule.PEN_THICKNESS}
            });
        }

        public override List<Rectangle> Selection(RoomData room, Entity entity)
        {
            return [new Rectangle(entity.x - 12, entity.y - 12, 24, 24)];
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("clockwise", true)
                .AddOptionsField("surface", "Floor", "Ceiling", "LeftWall", "RightWall", "Floor");
        }
    }
}