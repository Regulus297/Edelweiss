using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class KevinsPC : CSEntityData
    {
        public override string EntityName => "kevins_pc";

        public override List<string> PlacementNames()
        {
            return ["kevins_pc"];
        }

        public override int Depth(RoomData room, Entity entity) => 8999;

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Sprite computer = new Sprite("objects/kevinspc/pc", entity)
            {
                justificationY = 1
            };
            Sprite spectro = new Sprite("objects/kevinspc/spectrogram", entity)
            {
                justificationX = 0,
                justificationY = 0,
                sourceWidth = 32,
                sourceHeight = 18
            };

            spectro.x += -16;
            spectro.y += -39;


            return [computer, spectro];
        }
    }
}