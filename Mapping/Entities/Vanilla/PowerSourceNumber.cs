using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class PowerSourceNumber : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "powerSourceNumber";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override int Depth(RoomData room, Entity entity) => -10010;

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Sprite number = new Sprite("scenery/powersource_numbers/1", entity);
            Sprite glow = new Sprite("scenery/powersource_numbers/1_glow", entity);
            return [number, glow];
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("number", 1)
                .AddField("strawberries", "")
                .AddField("keys", "");
        }
    }
}