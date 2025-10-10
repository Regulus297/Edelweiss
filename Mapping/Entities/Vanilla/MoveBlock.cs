using System;
using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class MoveBlock : CSEntityData
    {
        public override string EntityName => "moveBlock";

        List<string> directions = ["Up", "Right", "Down", "Left"];

        public override List<string> PlacementNames()
        {
            List<string> placements = [];

            foreach (string dir in directions)
            {
                for (int steer = 0; steer < 2; steer++)
                {
                    for (int fast = 0; fast < 2; fast++)
                    {
                        placements.Add($"{dir}_{(steer == 0 ? "nosteer" : "steer")}_{(fast == 0 ? "slow" : "fast")}");
                    }
                }
            }

            return placements;
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            string[] split = placement.Split('_');
            return new Dictionary<string, object>()
            {
                {"width", 16},
                {"height", 16},
                {"direction", split[0]},
                {"canSteer", split[1] == "steer"},
                {"fast", split[2] == "fast"}
            };
        }

        public override int Depth(RoomData room, Entity entity) => 8995;

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            string direction = entity.Get("direction", "Up").ToLower();

            string blockTexture = "objects/moveBlock/base";
            bool steer = entity.Get<bool>("canSteer");
            bool fast = entity.Get<bool>("fast");

            if (steer)
            {
                blockTexture = direction switch
                {
                    "up" or "down" => "objects/moveBlock/base_v",
                    _ => "objects/moveBlock/base_h"
                };
            }

            NinePatch block = new NinePatch(blockTexture, entity.x, entity.y, entity.width, entity.height, "border", "repeat")
            {
                depth = entity.depth
            };

            Rect highlight = new Rect(entity.x + 2, entity.y + 2, entity.width - 4, entity.height - 4, EdelweissUtils.GetColor(59, 50, 101))
            {
                depth = entity.depth
            };
            Rect mid = new Rect(entity.x + 8, entity.y + 8, entity.width - 16, entity.height - 16, EdelweissUtils.GetColor(4, 3, 23))
            {
                depth = entity.depth
            };

            int arrowIndex = direction switch
            {
                "up" => 2,
                "left" => 4,
                "right" => 0,
                _ => 6
            };
            Sprite arrow = new Sprite($"objects/moveBlock/arrow{arrowIndex:00}", entity);
            arrow.x += entity.width / 2;
            arrow.y += entity.height / 2;

            int arrowX = entity.x + (entity.width - arrow.data.width) / 2;
            int arrowY = entity.y + (entity.height - arrow.data.height) / 2;
            Rect arrowRect = new Rect(arrowX, arrowY, arrow.data.width, arrow.data.height, fast ? "#bf0a1f" : highlight.color)
            {
                depth = entity.depth
            };
            List<Drawable> sprites = [highlight, mid];


            if (steer)
            {
                string buttonColor = EdelweissUtils.GetColor(71, 64, 112);
                const int buttonPopout = 3;

                // 3 in the original Loenn file: see https://github.com/Regulus297/Edelweiss/issues/2.
                const int buttonOffset = 4;

                if (direction == "up" || direction == "down")
                {
                    for (int y = 4; y <= entity.height - 4; y += 8)
                    {
                        int leftQuadX = y == 4 ? 16 : y == entity.height - 4 ? 0 : 8;
                        int rightQuadX = 16 - leftQuadX;
                        Sprite left = new Sprite("objects/moveBlock/button", entity)
                        {
                            sourceX = leftQuadX,
                            sourceWidth = 8,
                            sourceHeight = 8,
                            color = buttonColor,
                            rotation = -MathF.PI / 2,
                            justificationX = 0,
                            justificationY = 0
                        };
                        left.x -= buttonPopout;
                        left.y += y + buttonOffset;

                        Sprite right = new Sprite("objects/moveBlock/button", entity)
                        {
                            sourceX = rightQuadX,
                            sourceWidth = 8,
                            sourceHeight = 8,
                            color = buttonColor,
                            rotation = MathF.PI / 2,
                            justificationX = 0,
                            justificationY = 0
                        };
                        right.x += entity.width + buttonPopout;
                        right.y += y - buttonOffset;

                        sprites.Add(left);
                        sprites.Add(right);
                    }
                }
                else
                {
                    for (int x = 4; x <= entity.width - 4; x += 8)
                    {
                        int quadX = x == 4 ? 0 : x == entity.width - 4 ? 16 : 8;
                        Sprite button = new Sprite("objects/moveBlock/button", entity)
                        {
                            sourceX = quadX,
                            sourceWidth = 8,
                            sourceHeight = 8,
                            color = buttonColor,
                            justificationX = 0,
                            justificationY = 0
                        };
                        button.x += x - buttonOffset;
                        button.y -= buttonPopout;

                        sprites.Add(button);
                    }
                }
            }

            sprites.Add(block);
            sprites.Add(arrowRect);
            sprites.Add(arrow);

            return sprites;
        }

        public override bool Rotate(RoomData room, Entity entity, int rotation)
        {
            entity["direction"] = directions.Cycle(entity.Get("direction", "Up"), rotation);
            return true;
        }

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            return CycleBoolean(entity, "canSteer", amount);
        }

        public override JObject FieldInformation(string fieldName)
        {
            if (fieldName != "direction")
                return null;
            return new JObject()
            {
                {"items", JArray.FromObject(directions)}
            };
        }
    }
}