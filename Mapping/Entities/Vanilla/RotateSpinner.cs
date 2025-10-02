using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class RotateSpinner : CSEntityData
    {
        public override string EntityName => "rotateSpinner";

        private static readonly Dictionary<string, (bool, bool)> types = new Dictionary<string, (bool, bool)>()
        {
            {"blade", (false, false)},
            {"dust", (true, false)},
            {"star", (false, true)}
        };

        public override List<int> NodeLimits(RoomData room, Entity entity) => [1, 1];
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Circle;
        public override int Depth(RoomData room, Entity entity) => -50;
        public override List<string> PlacementNames()
        {
            List<string> placements = [];
            foreach (var item in types)
            {
                for (int i = 0; i < 2; i++)
                {
                    placements.Add($"{item.Key}_{(i == 0 ? "clockwise" : "anticlockwise")}");
                }
            }
            return placements;
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            string type = placement.Split('_')[0];
            string direction = placement.Split('_')[1];

            (bool dust, bool star) = types[type];
            return new Dictionary<string, object>()
            {
                {"clockwise", direction == "clockwise"},
                {"dust", dust},
                {"star", star}
            };
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            List<Drawable> sprites = [];

            if ((bool)entity["star"])
            {
                sprites.Add(new Sprite("danger/starfish13", entity));
            }
            else if ((bool)entity["dust"])
            {
                Sprite dustBase = new Sprite("danger/dustcreature/base00", entity);
                Sprite dustOutline = new Sprite("@Internal@/dust_creature_outlines/base00", entity)
                {
                    color = "#ff0000"
                };

                sprites.Add(dustBase);
                sprites.Add(dustOutline);
            }
            else
            {
                sprites.Add(new Sprite("danger/blade00", entity));
            }

            return sprites;
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            bool dust = (bool)entity["dust"];
            bool star = (bool)entity["star"];

            if (dust && !star)
            {
                entity["dust"] = false;
                entity["star"] = true;
            }
            else if (!dust && star)
            {
                entity["dust"] = false;
                entity["star"] = false;
            }
            else
            {
                entity["dust"] = true;
                entity["star"] = false;
                
            }

            return true;
        }
    }
}