using System;
using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal abstract class DashSwitch : CSEntityData
    {
        public abstract List<string> Directions { get; }
        public abstract string FieldName { get; }

        protected static readonly List<(string, string, bool)> directionLookup = [
            ("dashSwitchV", "ceiling", false),
            ("dashSwitchH", "leftSide", true),
            ("dashSwitchV", "ceiling", true),
            ("dashSwitchH", "leftSide", false),
        ];

        public override List<string> PlacementNames()
        {
            List<string> placements = [];
            foreach (string direction in Directions)
            {
                placements.Add($"{direction}_default");
                placements.Add($"{direction}_mirror");
            }
            return placements;
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"persistent", false},
                {"sprite", placement.Split('_')[1]},
                {"allGates", false},
                {FieldName, Directions.IndexOf(placement.Split('_')[0]) != 0}
            };
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            List<string> sprites = ["default", "mirror"];
            entity["sprite"] = sprites.Cycle(entity["sprite"].ToString(), amount);
            return true;
        }

        protected Sprite GetSprite(Entity entity)
        {
            return new Sprite(entity["sprite"].ToString() == "default" ? "objects/temple/dashButton00" : "objects/temple/dashButtonMirror00", entity);
        }

        protected bool RotateCommon(Entity entity, int sideIndex, int direction)
        {
            int targetIndex = (sideIndex + direction) % 4;
            targetIndex = (targetIndex + 4) % 4;

            if (sideIndex != targetIndex)
            {
                (string newName, string attribute, bool value) = directionLookup[targetIndex];

                entity.Name = newName;
                entity["ceiling"] = null;
                entity["leftSide"] = null;
                entity[attribute] = value;
            }

            return sideIndex != targetIndex;
        }
    }

    internal class DashSwitchH : DashSwitch
    {
        public override List<string> Directions => ["left", "right"];

        public override string FieldName => "leftSide";

        public override string EntityName => "dashSwitchH";

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            bool leftSide = (bool)entity["leftSide"];
            Sprite sprite = GetSprite(entity);

            if (leftSide)
            {
                sprite.y += 8;
                sprite.rotation = MathF.PI;
            }
            else
            {
                sprite.x += 8;
                sprite.y += 8;
                sprite.rotation = 0;
            }

            return [sprite];
        }

        public override bool Flip(RoomData room, Entity entity, bool horizontal, bool vertical)
        {
            if (horizontal)
            {
                entity["leftSide"] = !(bool)entity["leftSide"];
            }
            return horizontal;
        }

        public override bool Rotate(RoomData room, Entity entity, int rotation)
        {
            return RotateCommon(entity, (bool)entity["leftSide"] ? 1 : 3, rotation);
        }
    }

    internal class DashSwitchV : DashSwitch
    {
        public override List<string> Directions => ["up", "down"];

        public override string FieldName => "ceiling";

        public override string EntityName => "dashSwitchV";
        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            bool ceiling = (bool)entity["ceiling"];
            Sprite sprite = GetSprite(entity);

            if (ceiling)
            {
                sprite.x += 8;
                sprite.rotation = -MathF.PI / 2;
            }
            else
            {
                sprite.x += 8;
                sprite.y += 8;
                sprite.rotation = MathF.PI / 2;
            }

            return [sprite];
        }

        public override bool Flip(RoomData room, Entity entity, bool horizontal, bool vertical)
        {
            if (vertical)
            {
                entity["ceiling"] = !(bool)entity["ceiling"];
            }
            return vertical;
        }
        
        public override bool Rotate(RoomData room, Entity entity, int rotation)
        {
            return RotateCommon(entity, (bool)entity["ceiling"] ? 2 : 0, rotation);
        }
    }
}