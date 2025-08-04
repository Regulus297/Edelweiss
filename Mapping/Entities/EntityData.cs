using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities
{
    public abstract class EntityData
    {
        public abstract string Name { get; }
        public virtual string Texture(RoomData room, Entity entity)
        {
            return "";
        }

        public virtual List<float> Justification(RoomData room, Entity entity)
        {
            return [0.5f, 0.5f];
        }
    }
}