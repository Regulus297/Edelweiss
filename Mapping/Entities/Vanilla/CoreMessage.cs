using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class CoreMessage : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "everest/coreMessage";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override List<string> Mods()
        {
            return ["Everest"];
        }

        public override string Texture(RoomData room, Entity entity) => "@Internal@/core_message";

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddIntField("line", 0)
                .AddField("dialog", "app_ending")
                .AddField("outline", false);
        }
    }
}