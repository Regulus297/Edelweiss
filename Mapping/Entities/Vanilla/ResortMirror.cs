using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class ResortMirror : CSEntityData
    {
        public override string EntityName => "resortMirror";

        public override List<string> PlacementNames()
        {
            return ["default"];
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Sprite glass = new Sprite("objects/mirror/glassbreak00", entity);
            Sprite frame = new Sprite("objects/mirror/resortframe", entity);
            glass.depth = 9500;
            frame.depth = 9000;
            frame.justificationX = 0.5f;
            frame.justificationY = 1.0f;

            int quadX = (glass.atlasWidth - frame.atlasWidth + 2) / 2;
            int quadY = glass.atlasHeight - frame.atlasHeight + 8;
            glass.sourceX = quadX;
            glass.sourceY = quadY;
            glass.sourceWidth = frame.atlasWidth - 2;
            glass.sourceHeight = frame.atlasHeight - 8;
            glass.justificationX = glass.justificationY = 0;
            glass.x -= (frame.atlasWidth - 2) / 2;
            glass.y -= frame.atlasHeight - 8;
            return [glass, frame];
        }

        public override Rectangle Rectangle(RoomData room, Entity entity)
        {
            return new Sprite("objects/mirror/resortframe", entity)
            {
                justificationX = 0.5f,
                justificationY = 1.0f
            }.Bounds();
        }
    }
}