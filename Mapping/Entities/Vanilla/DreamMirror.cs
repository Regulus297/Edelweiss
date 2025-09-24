using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class DreamMirror : CSEntityData
    {
        public override string EntityName => "dreammirror";

        public override List<string> PlacementNames()
        {
            return ["normal"];
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Sprite frame = new Sprite("objects/mirror/frame", entity);
            frame.justificationY = 1.0f;
            frame.depth = 9000;

            Sprite glass = new Sprite("objects/mirror/glassbreak00", entity);
            glass.justificationY = 1.0f;
            glass.depth = 9500;

            return [frame, glass];
        }
    }
}