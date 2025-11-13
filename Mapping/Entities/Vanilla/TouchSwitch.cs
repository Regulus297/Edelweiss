using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class TouchSwitch : CSEntityData
    {
        public override string EntityName => "touchSwitch";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override int Depth(RoomData room, Entity entity) => 2000;

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            return [new Sprite("objects/touchswitch/container", entity), new Sprite("objects/touchswitch/icon00", entity)];
        }
    }
}