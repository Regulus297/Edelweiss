using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class LightningBreakerBox : CSEntityData
    {
        public override string EntityName => "lightningBlock";

        public override List<string> PlacementNames()
        {
            return ["breaker_box"];
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"flipX", false},
                {"music_progress", -1},
                {"music_session", false},
                {"music", ""},
                {"flag", false}
            };
        }

        public override List<float> Justification(RoomData room, Entity entity)
        {
            return [entity.Get<bool>("flipX") ? 0.75f : 0.25f, 0.25f];
        }

        public override int Depth(RoomData room, Entity entity) => -10550;
        public override string Texture(RoomData room, Entity entity) => "objects/breakerBox/Idle00";

        public override bool Flip(RoomData room, Entity entity, bool horizontal, bool vertical)
        {
            if (horizontal)
                entity["flipX"] = !entity.Get<bool>("flipX");
            return horizontal;
        }

        public override List<float> Scale(RoomData room, Entity entity)
        {
            return [entity.Get<bool>("flipX") ? -1 : 1, 1];
        }
    }
}