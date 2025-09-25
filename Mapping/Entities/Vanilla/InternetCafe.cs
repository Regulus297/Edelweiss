using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class InternetCafe : CSEntityData
    {
        public override string EntityName => "wavedashmachine";

        public override List<string> PlacementNames()
        {
            return ["cafe"];
        }

        public override int Depth(RoomData room, Entity entity) => 1000;

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Sprite back = new Sprite("objects/wavedashtutorial/building_back", entity);
            Sprite left = new Sprite("objects/wavedashtutorial/building_front_left", entity);
            Sprite right = new Sprite("objects/wavedashtutorial/building_front_right", entity);

            back.justificationY = left.justificationY = right.justificationY = 1.0f;
            return [back, left, right];
        }
    }
}