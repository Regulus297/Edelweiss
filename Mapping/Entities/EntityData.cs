using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities
{
    public abstract class EntityData
    {
        public abstract string Name { get; }
        public virtual string Texture(RoomData room, Entity entity)
        {
            return "";
        }

        public virtual List<Sprite> Sprite(RoomData room, Entity entity)
        {
            return [new(Texture(room, entity))];
        }

        public virtual void Draw(JArray shapes, RoomData room, Entity entity)
        {
            foreach (var sprite in Sprite(room, entity))
                shapes.Add(sprite.ToJObject());
        }

        public virtual List<float> Justification(RoomData room, Entity entity)
        {
            return [0.5f, 0.5f];
        }
    }
}