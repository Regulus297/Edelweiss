using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class SwitchGate : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "switchGate";

        public override List<string> PlacementNames()
        {
            return ["block", "mirror", "temple", "stars"];
        }

        public override List<int> NodeLimits(RoomData room, Entity entity) => [1, 1];
        public override NodeLineRenderType NodeLineRenderType(Entity entity) => Entities.NodeLineRenderType.Line;
        public override int Depth(RoomData room, Entity entity) => -9000;

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            string texture = entity.Get("sprite", "block");
            NinePatch block = new NinePatch($"objects/switchgate/{texture}", entity.x, entity.y, entity.width, entity.height)
            {
                depth = entity.depth
            };

            Sprite middle = new Sprite("objects/switchgate/icon00", entity);
            middle.x += entity.width / 2;
            middle.y += entity.height / 2;
            return [block, middle];
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddResizability(16, 16)
                .AddOptionsField("sprite", placement, [.. PlacementNames()])
                .AddField("persistent", false)
                .SetCyclableField("sprite");
        }
    }
}