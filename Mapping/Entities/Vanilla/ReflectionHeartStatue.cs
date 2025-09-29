using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;

// TODO: FIX NODES BEFORER FINISHING
namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class ReflectionHeartStatue : CSEntityData
    {
        public override string EntityName => "reflectionHeartStatue";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override int Depth(RoomData room, Entity entity) => 8999;
        public override Visibility NodeVisibility(Entity entity) => Visibility.Always;
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override List<int> NodeLimits(RoomData room, Entity entity) => [5, 5];

        private const string StatueTexture = "objects/reflectionHeart/statue";
        public const string TorchTexture = "objects/reflectionHeart/torch00";
        public const string GemTexture = "objects/reflectionHeart/gem";
        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            return [new Sprite(StatueTexture, entity) {
                justificationX = 0.5f,
                justificationY = 1.0f
            }];
        }

        public override List<Drawable> NodeSprite(RoomData room, Entity entity, int nodeIndex)
        {
            Point node = entity.GetNode(nodeIndex);
            if (nodeIndex < 4)
            {
                Sprite torch = new Sprite(TorchTexture, node)
                {
                    justificationX = 0,
                    justificationY = 0
                };
                torch.x -= 32;
                torch.y -= 64;

                Sprite hint = new Sprite($"objects/reflectionHeart/hint{nodeIndex:00}", node);
                hint.y += 28;

                return [torch, hint];
            }

            return [];
        }
    }
}