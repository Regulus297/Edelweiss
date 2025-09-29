using System;
using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;
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
        /// The name of the entity as displayed in Edelweiss
        /// </summary>
        public virtual string DisplayName => Name;

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
                y = entity.y,
                depth = Depth(room, entity),
                scaleX = Scale(room, entity)[0],
                scaleY = Scale(room, entity)[1],
                color = Color(room, entity)
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
            return new Rectangle(entity.x - (hasDimensions ? 0 : 2), entity.y - (hasDimensions ? 0 : 2), hasDimensions ? entity.width : 4, hasDimensions ? entity.height : 4);
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
        /// </summary>
        /// <param name="entity">The entity instance</param>
        public virtual NodeLineRenderType NodeLineRenderType(Entity entity)
        {
            return Entities.NodeLineRenderType.None;
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
        /// Returns the rectangle containing the node.
        /// Defaults to returning the entity's rectangle at the node's position
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        /// <param name="nodeIndex">The 0-based index of the node in the entity</param>
        public virtual Rectangle NodeRectangle(RoomData room, Entity entity, int nodeIndex)
        {
            if (nodeIndex >= entity.nodes.Count)
                return default;

            Point node = entity.nodes[nodeIndex];
            return Rectangle(room, entity).Translated(node.X, node.Y);
        }

        /// <summary>
        /// A pair of booleans determining whether the entity can be resized horizontally and vertically respectively.
        /// Each boolean defaults to true if the placement contains a definition for that axis and false if not
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        public virtual List<bool> CanResize(RoomData room, Entity entity)
        {
            return [GetPlacementData().ContainsKey("width"), GetPlacementData().ContainsKey("height")];
        }

        /// <summary>
        /// The minimum and maximum sizes an entity can have. Returns an array with the minimum width, minimum height, maximum width and maximum height.
        /// Defaults to [1, 1, int.MaxValue, int.MaxValue]
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        public virtual List<int> SizeBounds(RoomData room, Entity entity)
        {
            return [1, 1, int.MaxValue, int.MaxValue];
        }

        /// <summary>
        /// Determines when the entity's nodes should be rendered.
        /// Defaults to <see cref="Visibility.Selected"/>
        /// </summary>
        /// <param name="entity">The entity instance</param>
        public virtual Visibility NodeVisibility(Entity entity)
        {
            return Visibility.Selected;
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

        /// <summary>
        /// Rotates the entity
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        /// <param name="rotation">The amount of steps to rotate the entity. Positive means clockwise rotation, negative means anticlockwise rotation.</param>
        /// <returns>True if the entity is rotated, false if not</returns>
        public virtual bool Rotate(RoomData room, Entity entity, int rotation)
        {
            return false;
        }

        /// <summary>
        /// Flips the entity on the given axis
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        /// <param name="horizontal">True if the entity should be flipped horizontally</param>
        /// <param name="vertical">True if the entity should be flipped vertically</param>
        /// <returns>True if the flip changed the entity, false if not</returns>
        public virtual bool Flip(RoomData room, Entity entity, bool horizontal, bool vertical)
        {
            return false;
        }

        /// <summary>
        /// "Cycles" between different states of the entities (like colors of spinners, colors of cassette blocks, etc.)
        /// Edelweiss exclusive
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        /// <param name="amount">The amount to cycle this entity by</param>
        /// <returns>True if the entity was changed, false if not</returns>
        public virtual bool Cycle(RoomData room, Entity entity, int amount)
        {
            return false;
        }

        /// <summary>
        /// The rotation of the entity in degrees
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        public virtual float Rotation(RoomData room, Entity entity)
        {
            return 0f;
        }

        /// <summary>
        /// The scale of the entity
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        public virtual List<float> Scale(RoomData room, Entity entity)
        {
            return [1f, 1f];
        }

        /// <summary>
        /// The render depth of the entity. Positive values are behind the player and negative values are in front of the player. Defaults to 0.
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        public virtual int Depth(RoomData room, Entity entity)
        {
            return 0;
        }

        /// <summary>
        /// The list of mods associated with this entity
        /// </summary>
        public virtual List<string> Mods()
        {
            return [];
        }

        /// <summary>
        /// 
        /// </summary>
        public string ModsList => Mods().Count > 0 ? "[" + string.Join(" + ", Mods()) + "]" : "";

        /// <summary>
        /// Gets the selection rectangles for the entity and its nodes
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        public virtual List<Rectangle> Selection(RoomData room, Entity entity)
        {
            List<Rectangle> output = [];
            for (int i = -1; i < entity.nodes.Count; i++)
            {
                if (i == -1 && (entity.width != 0 || entity.height != 0))
                {
                    output.Add(new Rectangle(entity.x, entity.y, entity.width, entity.height));
                    continue;
                }
                output.Add(GetDefaultRectangle(room, entity, i));
            }
            return output;
        }

        /// <summary>
        /// Gets the default rectangle for the entity and the node index
        /// </summary>
        /// <param name="room">The room the entity is in</param>
        /// <param name="entity">The entity instance</param>
        /// <param name="nodeIndex">-1 for the main entity</param>
        public virtual Rectangle GetDefaultRectangle(RoomData room, Entity entity, int nodeIndex)
        {
            List<Drawable> sprites = nodeIndex == -1 ? Sprite(room, entity) : NodeSprite(room, entity, nodeIndex);
            if (sprites.Count == 0)
                return default;

            Rectangle output = sprites[0].Bounds();
            for (int i = 1; i < sprites.Count; i++)
            {
                output = output.Combine(sprites[i].Bounds());
            }
            if (nodeIndex == -1)
                return output;
            Point node = entity.nodes[nodeIndex];
            return output.Translated(node.X, node.Y);
        }
    }

    /// <summary>
    /// Determines how nodes are connected to the main entity
    /// </summary>
    public enum NodeLineRenderType
    {
        /// <summary>
        /// Nodes are not connected to the main entity
        /// </summary>
        None,

        /// <summary>
        /// Nodes are connected to the previous node in the chain
        /// </summary>
        Line,

        /// <summary>
        /// All nodes are connected directly to the main entity
        /// </summary>
        Fan,

        /// <summary>
        /// Nodes draw a circle around the main entity.
        /// </summary>
        Circle
    }

    /// <summary>
    /// Enum containing options for the visibility of an object
    /// </summary>
    public enum Visibility
    {
        /// <summary>
        /// Never visible
        /// </summary>
        Never,

        /// <summary>
        /// Only visible when the object is selected
        /// </summary>
        Selected,

        /// <summary>
        /// Always visible
        /// </summary>
        Always
    }
}