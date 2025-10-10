using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class SummitGemManager : CSEntityData
    {
        public override string EntityName => "summitGemManager";

        public override Visibility NodeVisibility(Entity entity) => Visibility.Always;

        public override List<string> PlacementNames()
        {
            return [];
        }

        public override string Texture(RoomData room, Entity entity) => "@Internal@/summit_gem_manager";
        public override List<Drawable> NodeSprite(RoomData room, Entity entity, int nodeIndex)
        {
            return [new Sprite($"collectables/summitgems/{nodeIndex}/gem00", entity.GetNode(nodeIndex)) {
                depth = -10010
            }];
        }
    }
}