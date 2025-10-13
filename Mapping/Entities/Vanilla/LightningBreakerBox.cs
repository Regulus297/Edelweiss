using System.Collections.Generic;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class LightningBreakerBox : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "lightningBlock";

        public override List<string> PlacementNames()
        {
            return ["breaker_box"];
        }

        public override List<float> Justification(RoomData room, Entity entity)
        {
            return [entity.Get<bool>("flipX") ? 0.75f : 0.25f, 0.25f];
        }

        public override int Depth(RoomData room, Entity entity) => -10550;
        public override string Texture(RoomData room, Entity entity) => "objects/breakerBox/Idle00";

        public override List<float> Scale(RoomData room, Entity entity)
        {
            return [entity.Get<bool>("flipX") ? -1 : 1, 1];
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("flipX", false)
                .AddField("music_progress", -1)
                .AddField("music_session", false)
                .AddField("music", "")
                .AddField("flag", false)
                .SetHorizontalFlipField("flipX");
        }
    }
}