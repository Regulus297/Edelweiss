using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Bonfire : CSEntityData
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

        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            string mode = entity.data["mode"].ToString().ToLower();
            entity.data["mode"] = mode switch
            {
                "lit" => "Smoking",
                "smoking" => "Unlit",
                _ => "Lit"
            };
            return true;
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"mode", "Lit"}
            };
        }

        public override JObject FieldInformation(string fieldName)
        {
            return new JObject()
            {
                {"items", new JArray() {
                    "Lit", "Smoking", "Unlit"
                }}
            };
        }
    }
}