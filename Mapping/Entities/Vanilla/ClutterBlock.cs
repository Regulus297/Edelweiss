using System;
using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;
using MoonSharp.Interpreter;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal abstract class ClutterBlock : CSEntityData
    {
        public abstract string ClutterColor { get; }

        public override List<string> PlacementNames()
        {
            return [ClutterColor];
        }

        protected string GetTexture(string color, int index)
        {
            return $"objects/resortclutter/{color}_{index:00}";
        }

        protected List<string> GetTextures(string color)
        {
            List<string> textures = [];
            int i = 0;
            while (true)
            {
                string texture = GetTexture(color, i);
                if (CelesteModLoader.GetTextureData("Gameplay/" + texture) == null)
                {
                    break;
                }
                textures.Add(texture);
                i++;
            }
            return textures;
        }

        protected bool SpriteFits(bool[,] filled, int x, int y, int w, int h)
        {
            if (x + w > filled.GetLength(0) || y + h > filled.GetLength(1))
                return false;
            for (int cx = x; cx < x + w; cx++)
            {
                for (int cy = y; cy < y + h; cy++)
                {
                    if (filled[cx, cy])
                        return false;
                }
            }
            return true;
        }

        protected void MarkComplete(bool[,] filled, int x, int y, int w, int h)
        {
            for (int cx = x; cx < Math.Min(x + w, filled.GetLength(0)); cx++)
            {
                for (int cy = y; cy < Math.Min(y + h, filled.GetLength(1)); cy++)
                {
                    filled[cx, cy] = true;
                }
            }
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            List<Drawable> sprites = [];

            int w = entity.width / 8;
            int h = entity.height / 8;
            bool[,] filled = new bool[w, h];

            List<string> textures = GetTextures(ClutterColor);
            Random random = new(room.x + room.y * entity.x - entity.y);
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    TextureData data;
                    string choice;
                    if (filled[x, y])
                        continue;
                    do
                    {
                        choice = textures[random.Next(0, textures.Count)];
                        data = CelesteModLoader.GetTextureData("Gameplay/" + choice);
                    } while (!SpriteFits(filled, x, y, data.width / 8, data.height / 8));

                    Sprite sprite = new(choice, entity);
                    sprite.justificationX = 0;
                    sprite.justificationY = 0;
                    sprite.x += x * 8 - 8;
                    sprite.y += y * 8 - 8;
                    sprites.Add(sprite);

                    MarkComplete(filled, x, y, data.width / 8, data.height / 8);
                }
            }

            return sprites;
        }

        public override int Depth(RoomData room, Entity entity) => -9998;
        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"width", 8},
                {"height", 8}
            };
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            entity.Name = entity.Name switch
            {
                "redBlocks" => "yellowBlocks",
                "yellowBlocks" => "greenBlocks",
                _ => "redBlocks"
            };
            return true;
        }
    }

    internal class RedClutterBlock : ClutterBlock
    {
        public override string ClutterColor => "red";

        public override string EntityName => "redBlocks";
    }

    internal class YellowClutterBlock : ClutterBlock
    {
        public override string ClutterColor => "yellow";

        public override string EntityName => "yellowBlocks";
    }

    internal class GreenClutterBlock : ClutterBlock
    {
        public override string ClutterColor => "green";

        public override string EntityName => "greenBlocks";
    }
}