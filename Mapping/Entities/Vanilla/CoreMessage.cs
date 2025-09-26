using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class CoreMessage : CSEntityData
    {
        public override string EntityName => "everest/coreMessage";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"line", 0},
                {"dialog", "app_ending"},
                {"outline", false}
            };
        }

        public override List<string> Mods()
        {
            return ["Everest"];
        }

        public override string Texture(RoomData room, Entity entity) => "@Internal@/core_message";
    }
}