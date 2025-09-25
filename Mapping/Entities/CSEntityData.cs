using System;
using System.Collections.Generic;
using System.Reflection;
using Edelweiss.Mapping.Drawables;
using Edelweiss.RegistryTypes;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities
{
    /// <summary>
    /// Base class for entity data defined in C#
    /// </summary>
    [BaseRegistryObject()]
    public abstract class CSEntityData : EntityData, IRegistryObject
    {
        /// <summary>
        /// The name of the placement for this specific entity
        /// </summary>
        protected string placement;

        /// <summary>
        /// The name of the entity
        /// </summary>
        public abstract string EntityName { get; }

        /// <inheritdoc/>
        public sealed override string Name => $"{EntityName}.{placement}";

        /// <inheritdoc/>
        public override string DisplayName => Language.GetTextOrDefault($"Edelweiss.entities.{Name}", Name) + " " + ModsList;

        /// <summary>
        /// Adds the entity data to the CelesteModLoader list
        /// </summary>
        public virtual void OnRegister()
        {
            foreach (string name in PlacementNames())
            {
                CSEntityData created = (CSEntityData)Activator.CreateInstance(GetType());
                created.placement = name;
                CelesteModLoader.AddEntity("vanilla", created, EntityName);
            }
        }

        /// <inheritdoc/>
        public override void Draw(JArray shapes, RoomData room, Entity entity)
        {
            if (Texture(room, entity) == "" && GetType().GetMethod("Sprite").DeclaringType == typeof(EntityData))
            {
                using var dest = new SpriteDestination(shapes, entity.x, entity.y);
                Rectangle(room, entity).Draw();
            }
            else
            {
                base.Draw(shapes, room, entity);
            }
        }

        /// <inheritdoc/>
        public override List<Drawable> NodeSprite(RoomData room, Entity entity, int nodeIndex)
        {
            if (NodeTexture(room, entity, nodeIndex) == "" && GetType().GetMethod("NodeTexture").DeclaringType == typeof(EntityData))
            {
                return Sprite(room, entity);
            }
            return base.NodeSprite(room, entity, nodeIndex);
        }

        /// <summary>
        /// The names of the placements this entity has
        /// </summary>
        /// <returns></returns>
        public abstract List<string> PlacementNames();

        /// <summary>
        /// Cycles a boolean value of an entity by the given amount
        /// </summary>
        /// <param name="entity">The entity being cycled</param>
        /// <param name="key">The field being cycled</param>
        /// <param name="amount">The amount to cycle by. If even, does nothing</param>
        protected bool CycleBoolean(Entity entity, string key, int amount)
        {
            if (amount.Mod(2) == 0)
            {
                return false;
            }
            entity[key] = !(bool)entity[key];
            return true;
        }
    }
}