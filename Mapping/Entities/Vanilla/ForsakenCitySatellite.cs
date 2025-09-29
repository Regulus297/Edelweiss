using System;
using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    // TODO: Fix node positions
    internal class ForsakenCitySatellite : CSEntityData
    {
        public override string EntityName => "birdForsakenCityGem";

        private const string BirdTexture = "scenery/flutterbird/flap01";
        private const string GemTexture = "collectables/heartGem/0/00";
        private const string DishTexture = "objects/citysatellite/dish";
        private const string LightTexture = "objects/citysatellite/light";
        private const string ComputerTexture = "objects/citysatellite/computer";
        private const string ScreenTexture = "objects/citysatellite/computerscreen";

        private const int ComputerOffsetX = 32, ComputerOffsetY = 24;
        private const int BirdFlightDistance = 32;

        private static readonly Dictionary<string, string> codeColors = new Dictionary<string, string>()
        {
            {"U", EdelweissUtils.GetColor(240, 240, 240)},
            {"DR", EdelweissUtils.GetColor(10, 68, 224)},
            {"UR", EdelweissUtils.GetColor(179, 45, 0)},
            {"L", EdelweissUtils.GetColor(145, 113, 242)},
            {"UL", EdelweissUtils.GetColor(255, 205, 55)}
        };

        public override List<string> PlacementNames()
        {
            return ["satellite"];
        }

        public override int Depth(RoomData room, Entity entity) => 8999;
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override List<int> NodeLimits(RoomData room, Entity entity) => [2, 2];

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Sprite dish = new Sprite(DishTexture, entity);
            dish.justificationY = 1.0f;

            Sprite light = new Sprite(LightTexture, entity);
            light.justificationY = 1.0f;

            Sprite computer = new Sprite(ComputerTexture, entity);
            computer.x += ComputerOffsetX;
            computer.y += ComputerOffsetY;

            Sprite screen = new Sprite(ScreenTexture, entity);
            screen.x += ComputerOffsetX;
            screen.y += ComputerOffsetY;

            return [dish, light, computer, screen];
        }

        public override List<Drawable> NodeSprite(RoomData room, Entity entity, int nodeIndex)
        {
            Point node = entity.GetNode(nodeIndex);
            if (nodeIndex == 0)
            {
                List<Drawable> sprites = [];
                foreach (KeyValuePair<string, string> codeColor in codeColors)
                {
                    string code = codeColor.Key;
                    string color = codeColor.Value;
                    int directionX = code.Contains('L') ? -1 : code.Contains('R') ? 1 : 0;
                    int directionY = code.Contains('U') ? -1 : code.Contains('D') ? 1 : 0;

                    int offsetX = directionX;
                    int offsetY = directionY;
                    float magnitude = MathF.Sqrt(offsetX * offsetX + offsetY * offsetY);

                    Sprite sprite = new Sprite(BirdTexture, node);
                    sprite.x += (int)(BirdFlightDistance * offsetX / magnitude);
                    sprite.y += (int)(BirdFlightDistance * offsetY / magnitude);

                    sprite.color = color;

                    if (offsetX == -1)
                    {
                        sprite.scaleX = -1;
                    }
                    sprites.Add(sprite);
                }
                return sprites;
            }

            return [new Sprite(GemTexture, node)];
        }

        public override Rectangle NodeRectangle(RoomData room, Entity entity, int nodeIndex)
        {
            Point node = entity.GetNode(nodeIndex);
            if (nodeIndex == 0)
                return new Rectangle(node.X - 8, node.Y - 8, 16, 16);

            return new Sprite(GemTexture, node).Bounds();
        }
    }
}