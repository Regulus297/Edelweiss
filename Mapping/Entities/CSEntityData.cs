using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;
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
                if(created is IFieldInfoEntity fieldInfoEntity)
                {
                    EntityFieldInfo fieldInfo = new();
                    fieldInfoEntity.InitializeFieldInfo(fieldInfo);
                    IFieldInfoEntity.fieldInfos[created.Name] = fieldInfo;
                }
                CelesteModLoader.AddEntity("vanilla", created, EntityName);
            }
        }

        /// <inheritdoc/>
        public override void Draw(JArray shapes, RoomData room, Entity entity)
        {
            if (Texture(room, entity) == "" && !MethodImplemented("Sprite"))
            {
                using var dest = new SpriteDestination(shapes, entity.x, entity.y);
                new Rect(Rectangle(room, entity), FillColor(room, entity), BorderColor(room, entity)).Draw();
            }
            else
            {
                base.Draw(shapes, room, entity);
            }
        }

        /// <inheritdoc/>
        public override void NodeDraw(JArray shapes, RoomData room, Entity entity, int nodeIndex)
        {
            if (!MethodImplemented(nameof(NodeTexture)) && !MethodImplemented(nameof(NodeSprite)))
            {
                Point node = entity.nodes[nodeIndex];
                using var dest = new SpriteDestination(null, -node.X, -node.Y);
                Draw(shapes, room, entity);
                return;
            }
            base.NodeDraw(shapes, room, entity, nodeIndex);
        }

        /// <inheritdoc/>
        public override Rectangle GetDefaultRectangle(RoomData room, Entity entity, int nodeIndex)
        {
            if (nodeIndex == -1 && MethodImplemented("Rectangle"))
                return Rectangle(room, entity);
            if (nodeIndex >= 0 && MethodImplemented("NodeRectangle"))
                return NodeRectangle(room, entity, nodeIndex);
            if (nodeIndex >= 0 && !MethodImplemented(nameof(NodeTexture)) && !MethodImplemented(nameof(NodeSprite))) {
                Point node = entity.nodes[nodeIndex];
                return GetDefaultRectangle(room, entity, -1).Translated(node.X, node.Y);
            }
            return base.GetDefaultRectangle(room, entity, nodeIndex);
        }

        private bool MethodImplemented(string method) => GetType().GetMethod(method).DeclaringType != typeof(EntityData);

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
            entity[key] = !entity.Get<bool>(key);
            return true;
        }
    }
}