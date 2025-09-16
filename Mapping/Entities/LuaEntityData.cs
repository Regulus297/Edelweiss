using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Plugins;
using Edelweiss.Utils;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities
{
    /// <summary>
    /// The class containing entity data for a Loenn entity
    /// </summary>
    /// <param name="name">The name of the entity</param>
    /// <param name="placementName">The name of the placement</param>
    /// <param name="placement">The placement table that this entity is for</param>
    /// <param name="script">The script the table belongs to</param>
    /// <param name="entityTable">The table containing all entity data</param>
    public class LuaEntityData(string name, string placementName, Table placement, Script script, Table entityTable) : EntityData
    {
        Script script = script;
        Table entityTable = entityTable;
        Table placement = placement;
        string name = name;
        string placementName = placementName;

        /// <inheritdoc/>
        public override string Name => name;
        /// <inheritdoc/>
        public override string DisplayName => Language.GetTextOrDefault($"Loenn.entities.{name}.placements.name.{placementName}") ?? placementName;

        /// <inheritdoc/>
        public override string Texture(RoomData room, Entity entity)
        {
            DynValue textureMethod = entityTable.Get("texture");
            if (textureMethod.IsNil())
            {
                return "";
            }
            if (textureMethod.Type == DataType.String)
            {
                return textureMethod.String;
            }
            return script.Call(textureMethod, room.ToLuaTable(script), entity.ToLuaTable(script)).String;
        }

        /// <inheritdoc/>
        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            DynValue spriteMethod = entityTable.Get("sprite");
            if (spriteMethod.IsNil())
                return base.Sprite(room, entity);
            DynValue sprite = script.Call(spriteMethod, room.ToLuaTable(script), entity.ToLuaTable(script));
            if (sprite.Table.Get("_type").IsNil())
            {
                List<Drawable> output = [];
                // It's a list
                foreach (var table in sprite.Table.Values)
                    output.Add(Drawable.FromTable(table.Table));
                return output;
            }
            // It's one sprite
            return [Drawable.FromTable(sprite.Table)];
        }

        /// <inheritdoc/>
        public override void Draw(JArray shapes, RoomData room, Entity entity)
        {
            try
            {
                DynValue drawMethod = entityTable.Get("draw");
                if (drawMethod.IsNil())
                {
                    if (entityTable.Get("texture").IsNil() && entityTable.Get("sprite").IsNil())
                    {
                        using var dest = new SpriteDestination(shapes, entity.x, entity.y);
                        Rectangle(room, entity).Draw();
                        return;
                    }
                    base.Draw(shapes, room, entity);
                }
                else
                {
                    using var dest = new SpriteDestination(shapes, entity.x, entity.y);
                    script.Call(drawMethod, room.ToLuaTable(script), entity.ToLuaTable(script), new Table(script));
                }
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while drawing entity {Name}: \n {e.Formatted()}");

                // If everything fails, draw the rectangle
                using var dest = new SpriteDestination(shapes, entity.x, entity.y);
                Rectangle(room, entity).Draw();
            }
        }


        /// <inheritdoc/>
        public override List<float> Justification(RoomData room, Entity entity)
        {
            try
            {
                DynValue justificationMethod = entityTable.Get("justification");
                if (justificationMethod.IsNil())
                {
                    return [0.5f, 0.5f];
                }
                if (justificationMethod.Type == DataType.Table)
                {
                    return justificationMethod.Table.Values.Select(v => (float)v.Number).ToList();
                }
                DynValue result = script.Call(justificationMethod, room.ToLuaTable(script), entity.ToLuaTable(script));
                if (result.Type == DataType.Table)
                    return result.Table.Values.Select(v => (float)v.Number).ToList();
                return result.Tuple.Select(n => (float)n.Number).ToList();
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while getting justification for entity {Name}: \n {e.Formatted()}");
                return [0.5f, 0.5f];
            }
        }

        /// <inheritdoc/>
        public override string Color(RoomData room, Entity entity)
        {
            try
            {
                DynValue colorMethod = entityTable.Get("color");
                if (colorMethod.IsNil())
                    return base.Color(room, entity);

                if (colorMethod.Type == DataType.Table)
                    return colorMethod.Color();
                return script.Call(colorMethod, room.ToLuaTable(script), entity.ToLuaTable(script)).Color();
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while getting colour for entity {Name}: \n {e.Formatted()}");
                return base.Color(room, entity);
            }
        }

        /// <inheritdoc/>
        public override string FillColor(RoomData room, Entity entity)
        {
            try
            {
                DynValue colorMethod = entityTable.Get("fillColor");
                if (colorMethod.IsNil())
                    return base.FillColor(room, entity);

                if (colorMethod.Type == DataType.Table)
                    return colorMethod.Color();
                if (colorMethod.Type == DataType.String)
                    return colorMethod.String;
                return script.Call(colorMethod, room.ToLuaTable(script), entity.ToLuaTable(script)).Color();
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while getting fill colour for entity {Name}: \n {e.Formatted()}");
                return base.FillColor(room, entity);
            }
        }

        /// <inheritdoc/>
        public override string BorderColor(RoomData room, Entity entity)
        {
            try
            {
                DynValue colorMethod = entityTable.Get("borderColor");
                if (colorMethod.IsNil())
                    return base.BorderColor(room, entity);

                if (colorMethod.Type == DataType.Table)
                    return colorMethod.Color();
                if (colorMethod.Type == DataType.String)
                    return colorMethod.String;
                return script.Call(colorMethod, room.ToLuaTable(script), entity.ToLuaTable(script)).Color();
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while getting border colour for entity {Name}: \n {e.Formatted()}");
                return base.FillColor(room, entity);
            }
        }

        /// <inheritdoc/>
        public override List<int> NodeLimits(RoomData room, Entity entity)
        {
            try
            {
                DynValue nodeLimitsMethod = entityTable.Get("nodeLimits");
                if (nodeLimitsMethod.IsNil())
                    return base.NodeLimits(room, entity);

                Table nodeLimits = nodeLimitsMethod.Table;
                if (nodeLimits == null)
                {
                    DynValue result = script.Call(nodeLimitsMethod, room.ToLuaTable(script), entity.ToLuaTable(script));
                    nodeLimits = result.Table;
                    if (nodeLimits == null && result.Type == DataType.Tuple)
                    {
                        nodeLimits = new Table(script, result.Tuple[0], result.Tuple[1]);
                    }
                }
                return [(int)(double)nodeLimits[1], (int)(double)nodeLimits[2]];
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while getting node limits for entity {Name}: \n {e.Formatted()}");
                return base.NodeLimits(room, entity);
            }
        }

        /// <inheritdoc/>
        public override NodeLineRenderType NodeLineRenderType(Entity entity)
        {
            try
            {
                DynValue method = entityTable.Get("nodeLineRenderType");
                if (method.IsNil())
                    return base.NodeLineRenderType(entity);

                DynValue result = method.Type == DataType.Function ? script.Call(method, entity.ToLuaTable(script)) : method;
                return result.Value() switch
                {
                    "line" => Entities.NodeLineRenderType.Line,
                    "fan" => Entities.NodeLineRenderType.Fan,
                    "circle" => Entities.NodeLineRenderType.Circle,
                    _ => Entities.NodeLineRenderType.None
                };
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while getting node line render type for entity {Name}: \n {e.Formatted()}");
                return base.NodeLineRenderType(entity);
            }
        }

        /// <inheritdoc/>
        public override Visibility NodeVisibility(Entity entity)
        {
            try
            {
                DynValue method = entityTable.Get("nodeVisibility");
                if (method.IsNil())
                    return base.NodeVisibility(entity);

                string result = method.Type == DataType.String ? method.String : script.Call(method, entity.ToLuaTable(script)).String;
                return result switch
                {
                    "never" => Visibility.Never,
                    "selected" => Visibility.Selected,
                    "always" => Visibility.Always,
                    _ => base.NodeVisibility(entity)
                };
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while getting node visibility for entity {Name}: \n {e.Formatted()}");
                return base.NodeVisibility(entity);
            }
        }

        /// <inheritdoc/>
        public override string NodeTexture(RoomData room, Entity entity, int nodeIndex)
        {
            DynValue textureMethod = entityTable.Get("nodeTexture");
            if (textureMethod.IsNil())
            {
                return base.NodeTexture(room, entity, nodeIndex);
            }
            if (textureMethod.Type == DataType.String)
            {
                return textureMethod.String;
            }
            var p = entity.nodes[nodeIndex];
            return script.Call(textureMethod, room.ToLuaTable(script), entity.ToLuaTable(script), entity.PointToTable(p, script), nodeIndex, new Table(script)).String;
        }

        /// <inheritdoc/>
        public override List<Drawable> NodeSprite(RoomData room, Entity entity, int nodeIndex)
        {
            DynValue spriteMethod = entityTable.Get("nodeSprite");
            // If sprite method is defined but no node texture, use that
            if (!entityTable.Get("sprite").IsNil() && entityTable.Get("nodeTexture").IsNil())
                return Sprite(room, entity);

            if (spriteMethod.IsNil())
                return base.NodeSprite(room, entity, nodeIndex);
            var p = entity.nodes[nodeIndex];
            DynValue sprite = script.Call(spriteMethod, room.ToLuaTable(script), entity.ToLuaTable(script), entity.PointToTable(p, script), nodeIndex, new Table(script));
            if (sprite.IsNil())
                return [];
            if (sprite.Table.Get("_type").IsNil())
            {
                List<Drawable> output = [];
                // It's a list
                foreach (var table in sprite.Table.Values)
                    output.Add(Drawable.FromTable(table.Table));
                return output;
            }
            // It's one sprite
            return [Drawable.FromTable(sprite.Table)];
        }

        /// <inheritdoc/>
        public override void NodeDraw(JArray shapes, RoomData room, Entity entity, int nodeIndex)
        {
            try
            {
                DynValue drawMethod = entityTable.Get("nodeDraw");
                if (drawMethod.IsNil())
                {
                    base.NodeDraw(shapes, room, entity, nodeIndex);
                }
                else
                {
                    using var dest = new SpriteDestination(shapes, entity.x, entity.y);
                    Point p = entity.nodes[nodeIndex];
                    script.Call(drawMethod, room.ToLuaTable(script), entity.ToLuaTable(script), entity.PointToTable(p, script), nodeIndex, new Table(script));
                }
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while drawing node for entity {Name}: \n {e.Formatted()}");

                // If everything fails, draw the rectangle
                using var dest = new SpriteDestination(shapes, entity.x, entity.y);
                Rectangle(room, entity).Draw();
            }
        }

        /// <inheritdoc/>
        public override List<bool> CanResize(RoomData room, Entity entity)
        {
            try
            {
                DynValue canResizeMethod = entityTable.Get("canResize");
                if (canResizeMethod.IsNil())
                    return base.CanResize(room, entity);

                Table canResize = canResizeMethod.Type == DataType.Table ? canResizeMethod.Table : script.Call(canResizeMethod, room.ToLuaTable(script), entity.ToLuaTable(script)).Table;
                return [(bool)canResize[1], (bool)canResize[2]];
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while getting resizability for entity {Name}: \n {e.Formatted()}");
                return base.CanResize(room, entity);
            }
        }

        /// <inheritdoc/>
        public override List<int> SizeBounds(RoomData room, Entity entity)
        {
            try
            {
                DynValue minSizeMethod = entityTable.Get("minimumSize");
                DynValue maxSizeMethod = entityTable.Get("maximumSize");

                List<int> defaultSize = base.SizeBounds(room, entity);
                List<int> size = [0, 0, 0, 0];
                if (minSizeMethod.IsNil())
                {
                    size[0] = defaultSize[0];
                    size[1] = defaultSize[1];
                }
                else
                {
                    Table minSizeTable = minSizeMethod.Type == DataType.Table ? minSizeMethod.Table : script.Call(minSizeMethod, room.ToLuaTable(script), entity.ToLuaTable(script)).Table;
                    size[0] = (int)(double)minSizeTable[1];
                    size[1] = (int)(double)minSizeTable[2];
                }

                if (maxSizeMethod.IsNil())
                {
                    size[2] = defaultSize[2];
                    size[3] = defaultSize[3];
                }
                else
                {
                    Table maxSizeTable = maxSizeMethod.Type == DataType.Table ? maxSizeMethod.Table : script.Call(maxSizeMethod, room.ToLuaTable(script), entity.ToLuaTable(script)).Table;
                    size[2] = (int)(double)maxSizeTable[1];
                    size[3] = (int)(double)maxSizeTable[2];
                }
                return size;
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while getting size bounds for entity {Name}: \n {e.Formatted()}");
                return base.SizeBounds(room, entity);
            }
        }

        /// <inheritdoc/>
        public override Dictionary<string, object> GetPlacementData()
        {
            try
            {
                Dictionary<string, object> data = [];
                Table dataTable = placement.Get("data").Table;
                if (dataTable == null)
                    return [];
                foreach (var value in dataTable.Keys)
                {
                    data[value.String] = dataTable.Get(value).ToObject();
                }
                return data;
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while getting placement data for entity {Name}: \n {e.Formatted()}");
                return [];
            }
        }
    }
}