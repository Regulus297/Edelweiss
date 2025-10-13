using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class NPC : CSEntityData
    {
        public override string EntityName => "npc";
        public override int Depth(RoomData room, Entity entity) => 100;
        public override List<float> Justification(RoomData room, Entity entity) => [0.5f, 1.0f];

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override string Texture(RoomData room, Entity entity) => entity.Get("npc", "granny_00_house").Split('_')[0] switch
        {
            "theo" => "characters/theo/theo00",
            "oshiro" => "characters/oshiro/oshiro24",
            "evil" or "badeline" => "characters/badeline/sleep00",
            _ => "characters/oldlady/idle00"
        };
    }
    internal class EverestNPC : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "everest/npc";

        public override List<string> PlacementNames()
        {
            return ["npc"];
        }

        public override List<string> Mods()
        {
            return ["Everest"];
        }

        public override List<float> Justification(RoomData room, Entity entity) => [0.5f, 1.0f];
        public override int Depth(RoomData room, Entity entity) => 100;
        public override string Texture(RoomData room, Entity entity) => $"characters/{entity.Get<string>("sprite")}00";
        public override List<float> Scale(RoomData room, Entity entity)
        {
            return [entity.Get<bool>("flipX") ? -1 : 1, entity.Get<bool>("flipY") ? -1 : 1];
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("sprite", "player/idle")
                .AddField("spriteRate", 1)
                .AddField("dialogId", "")
                .AddField("onlyOnce", true)
                .AddField("endLevel", false)
                .AddField("flipX", false)
                .AddField("flipY", false)
                .AddField("approachWhenTalking", false)
                .AddField("approachDistance", 16)
                .AddField("indicatorOffsetX", 0)
                .AddField("indicatorOffsetY", 0)
                .SetHorizontalFlipField("flipX")
                .SetVerticalFlipField("flipY");
        }
    }
}