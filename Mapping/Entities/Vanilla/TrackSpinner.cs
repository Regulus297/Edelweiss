using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class TrackSpinner : CSEntityData, IFieldInfoEntity
    {
        private static readonly Dictionary<string, (bool, bool)> Types = new Dictionary<string, (bool, bool)>() {
            {"blade", (false, false)},
            {"dust", (true, false)},
            {"starfish", (false, true)}
        };

        private static readonly List<string> Speeds = ["Slow", "Normal", "Fast"];

        public override string EntityName => "trackSpinner";

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            string speed = placement.Split('_')[0];
            string type = placement.Split('_')[1];
            (bool dust, bool star) = Types[type];
            fieldInfo.AddOptionsField("speed", speed, "Slow", "Normal", "Fast")
                .AddField("dust", dust)
                .AddField("star", star)
                .AddField("startCenter", false);
        }

        public override List<string> PlacementNames()
        {
            List<string> placements = [];
            foreach (string speed in Speeds)
            {
                foreach (string type in Types.Keys)
                {
                    placements.Add($"{speed}_{type}");
                }
            }
            return placements;
        }

        public override List<int> NodeLimits(RoomData room, Entity entity) => [1, 1];
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override int Depth(RoomData room, Entity entity) => -50;
        private List<Drawable> GetSprite(Entity entity)
        {
            List<Drawable> sprites = [];

            if (entity.Get("star", false))
            {
                sprites.Add(new Sprite("danger/starfish13", entity));
            }
            else if (entity.Get("dust", false))
            {
                sprites.Add(new Sprite("danger/dustcreature/base00", entity));
                sprites.Add(new Sprite("@Internal@/dust_creature_outlines/base00", entity)
                {
                    color = "#ff0000"
                });
            }
            else
            {
                sprites.Add(new Sprite("danger/blade00", entity));
            }

            return sprites;
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            return GetSprite(entity);
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            bool dust = entity.Get<bool>("dust");
            bool star = entity.Get<bool>("star");

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