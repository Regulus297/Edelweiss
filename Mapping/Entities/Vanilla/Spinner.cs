using System;
using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Spinner : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "spinner";

        public override List<string> PlacementNames()
        {
            return ["dust", "blue", "red", "purple", "core", "rainbow"];
        }

        private static string GetSpinnerTexture(string color, bool foreground)
        {
            return $"danger/crystal/{(foreground ? "fg_" : "bg_")}{color}00";
        }

        private static Sprite GetSpinnerSprite(Entity entity, bool foreground)
        {
            string color = entity.Get("color", "blue");
            color = color == "rainbow" ? "white" : color == "core" ? "red" : color;
            string texture = GetSpinnerTexture(color, foreground);
            return new Sprite(texture, entity);
        }

        private static List<Drawable> GetConnectionSprites(RoomData room, Entity entity)
        {
            List<Drawable> sprites = [];

            foreach (Entity other in room.entities)
            {
                if (other.EntityName == entity.EntityName && !other.Get<bool>("dust") && other.Get<bool>("attachToSolid") == entity.Get<bool>("attachToSolid"))
                {
                    if (new Point(other.x, other.y).Distance(new Point(entity.x, entity.y)) < 24)
                    {
                        Sprite sprite = GetSpinnerSprite(entity, false);
                        sprite.depth = -0;
                        sprite.x = (entity.x + other.x) / 2;
                        sprite.y = (entity.y + other.y) / 2;
                        sprites.Add(sprite);
                    }
                }
            }

            return sprites;
        }

        public override int Depth(RoomData room, Entity entity) => entity.Get<bool>("dust") ? -50 : -8500;

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            if (entity.Get<bool>("dust"))
            {
                Sprite main = new Sprite("danger/dustcreature/base00", entity)
                {
                    depth = 0
                };
                Sprite outline = new Sprite("@Internal@/dust_creature_outlines/base00", entity)
                {
                    color = "#ff0000"
                };
                return [main, outline];
            }

            List<Drawable> sprites = GetConnectionSprites(room, entity);
            sprites.Add(GetSpinnerSprite(entity, true));
            return sprites;
        }

        public override List<Rectangle> Selection(RoomData room, Entity entity)
        {
            if (entity.Get<bool>("dust"))
                return [new Sprite("danger/dustcreature/base00", entity).Bounds()];
            return [new Rectangle(entity.x - 8, entity.y - 8, 16, 16)];
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            List<string> colors = ["blue", "red", "purple", "core", "rainbow"];
            entity["color"] = colors.Cycle(entity.Get("color", "blue"), amount);
            return true;
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddOptionsField("color", placement == "dust" ? "blue" : placement, ("Blue", "blue"), ("Red", "red"), ("Purple", "purple"), ("Core", "core"), ("Rainbow", "rainbow"))
                .AddField("dust", placement == "dust")
                .AddField("attachToSolid", false)
                .SetCyclableField("color");
        }
    }
}