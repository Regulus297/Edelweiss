using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities
{
    /// <summary>
    /// Base class for entity data
    /// </summary>
    public abstract class EntityData
    {
        /// <summary>
        /// The name of the entity
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The texture key of the entity, relative to Gameplay/
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        public virtual string Texture(RoomData room, Entity entity)
        {
            return "";
        }

        /// <summary>
        /// Gets the sprites for the entity. Defaults to returning a sprite with the texture key returned by <see cref="Texture"/>.
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        public virtual List<Sprite> Sprite(RoomData room, Entity entity)
        {
            return [new(Texture(room, entity)) {
                justificationX = Justification(room, entity)[0],
                justificationY = Justification(room, entity)[1]
            }];
        }

        /// <summary>
        /// Draws the entity to the shapes array.
        /// Defaults to drawing the sprites returned by <seealso cref="Sprite"/> onto the array.
        /// </summary>
        /// <param name="shapes">The list of shapes this entity will be made up of</param>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        public virtual void Draw(JArray shapes, RoomData room, Entity entity)
        {
            using var dest = new SpriteDestination(shapes, entity.x, entity.y);
            foreach (var sprite in Sprite(room, entity))
                sprite.Draw();
        }

        /// <summary>
        /// Gets the fields of the entity from the placement
        /// </summary>
        public virtual Dictionary<string, object> GetPlacementData()
        {
            return [];
        }

        /// <summary>
        /// A list of two floats ranging from 0 to 1. Determines the position of the sprite relative to the entity's position.
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        public virtual List<float> Justification(RoomData room, Entity entity)
        {
            return [0.5f, 0.5f];
        }
    }
}