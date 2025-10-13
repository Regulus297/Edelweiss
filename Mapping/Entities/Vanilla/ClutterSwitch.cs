using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class ClutterSwitch : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "colorSwitch";

        public override List<string> PlacementNames()
        {
            return ["red", "green", "yellow", "lightning"];
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Sprite buttonSprite = new Sprite("objects/resortclutter/clutter_button00", entity);
            Sprite clutterSprite = new Sprite($"objects/resortclutter/icon_{entity.Get("type", "red")}", entity);

            buttonSprite.x += 16;
            buttonSprite.y += 16;
            buttonSprite.justificationX = 0.5f;
            buttonSprite.justificationY = 1.0f;

            clutterSprite.x += 16;
            clutterSprite.y += 8;
            clutterSprite.justificationX = 0.5f;
            clutterSprite.justificationY = 0.5f;

            return [buttonSprite, clutterSprite];
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddOptionsField("type", placement, "red", "green", "yellow", "lightning")
                .SetCyclableField("type");
        }
    }
}