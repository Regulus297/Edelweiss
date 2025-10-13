using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Memorial : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "everest/memorial";

        public override List<string> PlacementNames()
        {
            return ["memorial"];
        }

        public override string Texture(RoomData room, Entity entity)
        {
            return entity.Get("sprite", "scenery/memorial/memorial");
        }

        public override List<string> Mods()
        {
            return ["Everest"];
        }

        public override int Depth(RoomData room, Entity entity) => 100;
        public override List<float> Justification(RoomData room, Entity entity) => [0.5f, 1.0f];

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("dialog", "MEMORIAL")
                .AddField("sprite", "scenery/memorial/memorial")
                .AddField("spacing", 16);
        }
    }
}