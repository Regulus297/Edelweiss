using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Bonfire : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "bonfire";

        public override List<string> PlacementNames()
        {
            return ["bonfire"];
        }

        public override int Depth(RoomData room, Entity entity) => -5;
        public override List<float> Justification(RoomData room, Entity entity) => [0.5f, 1.0f];

        public override string Texture(RoomData room, Entity entity)
        {
            string mode = entity.data["mode"].ToString().ToLower();
            return mode switch
            {
                "lit" => "objects/campfire/fire08",
                "smoking" => "objects/campfire/smoking04",
                _ => "objects/campfire/fire00"
            };
        }


        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddOptionsField("mode", "Lit", "Lit", "Smoking", "Unlit")
                .SetCyclableField("mode");
        }
    }
}