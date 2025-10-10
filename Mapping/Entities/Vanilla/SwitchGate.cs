using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class SwitchGate : CSEntityData
    {
        public override string EntityName => "switchGate";

        public override List<string> PlacementNames()
        {
            return ["block", "mirror", "temple", "stars"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"width", 16},
                {"height", 16},
                {"sprite", placement},
                {"persistent", false}
            };
        }

        public override List<int> NodeLimits(RoomData room, Entity entity) => [1, 1];
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override int Depth(RoomData room, Entity entity) => -9000;

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            string texture = entity.Get("sprite", "block");
            NinePatch block = new NinePatch($"objects/switchgate/{texture}", entity.x, entity.y, entity.width, entity.height)
            {
                depth = entity.depth
            };

            Sprite middle = new Sprite("objects/switchgate/icon00", entity);
            middle.x += entity.width / 2;
            middle.y += entity.height / 2;
            return [block, middle];
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            entity["sprite"] = PlacementNames().Cycle(entity.Get("sprite", "block"), amount);
            return true;
        }

        public override JObject FieldInformation(string fieldName)
        {
            if (fieldName != "sprite")
                return null;

            return new JObject()
            {
                {"items", JArray.FromObject(PlacementNames())}
            };
        }
    }
}