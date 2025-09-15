using System;
using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
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
        public virtual List<Drawable> Sprite(RoomData room, Entity entity)
        {
            return [new Sprite(Texture(room, entity)) {
                justificationX = Justification(room, entity)[0],
                justificationY = Justification(room, entity)[1],
                x = entity.x,
                y = entity.y
            }];
        }

        /// <summary>
        /// Draws the entity to the shapes array.
        /// Defaults to drawing the sprites returned by <see cref="Sprite"/> onto the array.
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
        /// Returns the rectangle that contains the entity.
        /// Defaults to returning a rectangle with the width and height of the entity.
        /// If these are zero, returns a 4px * 4px rectangle centered on the entity's position.
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        public virtual Rectangle Rectangle(RoomData room, Entity entity)
        {
            bool hasDimensions = entity.width != 0 && entity.height != 0;
            return new Rectangle()
            {
                x = entity.x - (hasDimensions ? 0 : 2),
                y = entity.y - (hasDimensions ? 0 : 2),
                width = hasDimensions ? entity.width : 4,
                height = hasDimensions ? entity.height : 4,
                color = FillColor(room, entity),
                borderColor = BorderColor(room, entity)
            };
        }

        /// <summary>
        /// Returns the hex representation of the color of the entity.
        /// Defaults to white.
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        public virtual string Color(RoomData room, Entity entity)
        {
            return "#ffffff";
        }

        /// <summary>
        /// Returns the hex representation for the fill color of the entity.
        /// Defaults to returning <see cref="Color"/> if defined.
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        public virtual string FillColor(RoomData room, Entity entity)
        {
            return Color(room, entity);
        }


        /// <summary>
        /// Returns the hex representation for the border color of the entity.
        /// Defaults to returning <see cref="Color"/> if defined.
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        public virtual string BorderColor(RoomData room, Entity entity)
        {
            return Color(room, entity);
        }

        /// <summary>
        /// The minimum and maximum number of nodes an entity can have.
        /// A maximum of -1 means the entity does not have an upper limit.
        /// Defaults to [0, 0]
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        public virtual List<int> NodeLimits(RoomData room, Entity entity)
        {
            return [0, 0];
        }

        /// <summary>
        /// Determines how nodes are connected to the main entity. <br/>
        /// none: nodes are not connected to the main entity. <br/>
        /// line: nodes are connected to the previous node <br/>
        /// fan: all nodes are connected directly to the main entity <br/>
        /// circle: nodes draw a circle around the main entity. <br/>
        /// In Edelweiss, circle supports multiple nodes, not just one. Each will draw a circle around the entity. <br/>
        /// Defaults to none.
        /// </summary>
        /// <param name="entity">The entity instance</param>
        public virtual string NodeLineRenderType(Entity entity)
        {
            return "none";
        }
        
        /// <summary>
        /// The texture of the node, relative to Gameplay/
        /// Defaults to returning the value returned by <see cref="Texture"/>
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        /// <param name="nodeIndex">The 0-based index of the node in the entity</param> 
        public virtual string NodeTexture(RoomData room, Entity entity, int nodeIndex)
        {
            return Texture(room, entity);
        }

        /// <summary>
        /// Gets the sprites for the node. Defaults to returning a sprite with the texture key returned by <see cref="NodeTexture"/> 
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        /// <param name="nodeIndex">The 0-based index of the node in the entity</param>
        public virtual List<Drawable> NodeSprite(RoomData room, Entity entity, int nodeIndex)
        {
            return [new Sprite(NodeTexture(room, entity, nodeIndex)) {
                justificationX = Justification(room, entity)[0],
                justificationY = Justification(room, entity)[1],
                x = entity.x,
                y = entity.y
            }];
        }

        /// <summary>
        /// Draws the node onto the shapes array.
        /// Defaults to drawing the sprites returned by <see cref="NodeSprite"/> onto the array.
        /// </summary>
        /// <param name="shapes">The list of shapes the entity will be drawn to</param>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        /// <param name="nodeIndex">The 0-based index of the node in the entity</param>
        public virtual void NodeDraw(JArray shapes, RoomData room, Entity entity, int nodeIndex)
        {
            using var dest = new SpriteDestination(shapes, entity.x, entity.y);
            foreach (var sprite in NodeSprite(room, entity, nodeIndex))
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