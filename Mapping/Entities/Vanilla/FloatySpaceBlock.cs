using System.Collections.Generic;
using System.Linq;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class FloatySpaceBlock : CSEntityData
    {
        public override string EntityName => "floatySpaceBlock";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"tiletype", TileHelper.GetMaterial("m")},
                {"disableSpawnOffset", false},
                {"width", 8},
                {"height", 8}
            };
        }

        // TODO: fancy merge stuff
        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            List<Entity> entities = room.entities.Where(e => e.Value.Name == entity.Name && e.Value.Get<bool>("tiletype") == entity.Get<bool>("tiletype")).Select(e => e.Value).ToList();

            if (entities.Count <= 1 || !entities.Contains(entity))
                return TileHelper.GetSprite(entity, "tiletype");

            if (entities[0] == entity)
            {
                return TileHelper.GetMergedSprite(entities, "tiletype");
            }
            return [];
        }

        public override bool Cycle(RoomData room, Entity entity, int amount) => TileHelper.Cycle(entity, "tiletype", amount);
        public override JObject FieldInformation(string fieldName) => TileHelper.GetFieldInformation(fieldName, "tiletype");
    }
}