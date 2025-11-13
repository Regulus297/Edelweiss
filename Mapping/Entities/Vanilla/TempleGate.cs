using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class TempleGate : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "templeGate";

        private static readonly List<(string, string)> placementPresets = new List<(string, string)>()
        {
            ("Theo", "HoldingTheo"),
            ("Default", "CloseBehindPlayer"),
            ("Mirror", "CloseBehindPlayer"),
            ("Default", "NearestSwitch"),
            ("Mirror", "NearestSwitch"),
            ("Default", "TouchSwitches")
        };

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("height", 48)
                .AddOptionsField("sprite", placement.Split('_')[0], "Default", "Mirror", "Theo")
                .AddOptionsField("type", placement.Split('_')[1], "NearestSwitch", "CloseBehindPlayer", "CloseBehindPlayerAlways", "TouchSwitches", "HoldingTheo", "CloseBehindPlayerAndTheo")
                .SetCyclableField("sprite");
        }

        public override List<string> PlacementNames()
        {
            return placementPresets.Select(x => $"{x.Item1}_{x.Item2}").ToList();
        }

        public override List<bool> CanResize(RoomData room, Entity entity) => [false, false];
        public override int Depth(RoomData room, Entity entity) => -9000;

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            string texture = entity.Get("sprite", "default") switch
            {
                "Default" => "objects/door/TempleDoor00",
                "Mirror" => "objects/door/TempleDoorB00",
                _ => "objects/door/TempleDoorC00"
            };

            Sprite sprite = new Sprite(texture, entity)
            {
                justificationY = 0
            };
            sprite.x += 4;
            sprite.y += entity.height - 48;
            return [sprite];
        }

        public override List<Rectangle> Selection(RoomData room, Entity entity)
        {
            return [GetDefaultRectangle(room, entity, -1)];
        }
    }
}