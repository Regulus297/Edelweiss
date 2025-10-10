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
    /// <param name="modName">The name of the mod defining this entity</param>
    public class LuaEntityData(string name, string placementName, Table placement, Script script, Table entityTable, string modName) : EntityData
    {
        Script script = script;
        Table entityTable = entityTable;
        Table placement = placement;
        string name = name;
        string placementName = placementName;
        string modName = modName;

        /// <inheritdoc/>
        public override string Name => $"{name}.{placementName}";

        /// <inheritdoc/>
        public override string DisplayName => (Language.GetTextOrDefault($"Loenn.entities.{name}.placements.name.{placementName}") ?? Name) + " " + ModsList;

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
                        new Rect(Rectangle(room, entity), FillColor(room, entity), BorderColor(room, entity)).Draw();
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
                new Rect(Rectangle(room, entity), FillColor(room, entity), BorderColor(room, entity)).Draw();
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
            return script.Call(textureMethod, room.ToLuaTable(script), entity.ToLuaTable(script), entity.GetNode(nodeIndex).ToLuaTable(script), nodeIndex + 1, new Table(script)).String;
        }

        /// <inheritdoc/>
        public override List<Drawable> NodeSprite(RoomData room, Entity entity, int nodeIndex)
        {
            DynValue spriteMethod = entityTable.Get("nodeSprite");

            if (spriteMethod.IsNil())
                return base.NodeSprite(room, entity, nodeIndex);

            DynValue sprite = script.Call(spriteMethod, room.ToLuaTable(script), entity.ToLuaTable(script), entity.GetNode(nodeIndex).ToLuaTable(script), nodeIndex + 1, new Table(script));
            if (sprite.IsNil())
            {
                return [];
            }
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
                    // If no nodeSprite method or nodeTexture method is defined, use the draw method for the main entity
                    if (entityTable.Get("nodeSprite").IsNil() && entityTable.Get("nodeTexture").IsNil())
                    {
                        var node = entity.nodes[nodeIndex];
                        using var dest = new SpriteDestination(null, -node.X, -node.Y);
                        Draw(shapes, room, entity);
                        return;
                    }
                    base.NodeDraw(shapes, room, entity, nodeIndex);
                }
                else
                {
                    using var dest = new SpriteDestination(shapes, entity.x, entity.y);
                    script.Call(drawMethod, room.ToLuaTable(script), entity.ToLuaTable(script), entity.GetNode(nodeIndex).ToLuaTable(script), nodeIndex + 1, new Table(script));
                }
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while drawing node for entity {Name}: \n {e.Formatted()}");

                // If everything fails, draw the rectangle
                using var dest = new SpriteDestination(shapes, entity.x, entity.y);
                new Rect(Rectangle(room, entity), FillColor(room, entity), BorderColor(room, entity)).Draw();
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

        /// <inheritdoc/>
        public override bool Rotate(RoomData room, Entity entity, int rotation)
        {
            try
            {
                DynValue rotateMethod = entityTable.Get("rotate");
                if (rotateMethod.IsNil())
                    return base.Rotate(room, entity, rotation);
                DynValue result = script.Call(rotateMethod, room.ToLuaTable(script), entity.ToLuaTable(script), DynValue.NewNumber(rotation));
                return result.Boolean;
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while rotating entity {Name}: \n {e.Formatted()}");
                return base.Rotate(room, entity, rotation);
            }
        }

        /// <inheritdoc/>
        public override bool Flip(RoomData room, Entity entity, bool horizontal, bool vertical)
        {
            try
            {
                DynValue flipMethod = entityTable.Get("flip");
                if (flipMethod.IsNil())
                    return base.Flip(room, entity, horizontal, vertical);
                DynValue result = script.Call(flipMethod, room.ToLuaTable(script), entity.ToLuaTable(script), DynValue.NewBoolean(horizontal), DynValue.NewBoolean(vertical));
                return result.Boolean;
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while flipping entity {Name}: \n {e.Formatted()}");
                return base.Flip(room, entity, horizontal, vertical);
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Implementend in lua as entity.cycle(room, entity, amount)
        /// </remarks>
        public override bool Cycle(RoomData room, Entity entity, int amount)
        {
            try
            {
                DynValue cycleMethod = entityTable.Get("cycle");
                if (cycleMethod.IsNil())
                    return base.Cycle(room, entity, amount);
                DynValue result = script.Call(cycleMethod, room.ToLuaTable(script), entity.ToLuaTable(script), DynValue.NewNumber(amount));
                return result.Boolean;
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while cycling entity {Name}: \n {e.Formatted()}");
                return base.Cycle(room, entity, amount);
            }
        }

        /// <inheritdoc/>
        public override float Rotation(RoomData room, Entity entity)
        {
            try
            {
                DynValue rotationMethod = entityTable.Get("rotation");
                if (rotationMethod.IsNil())
                    return base.Rotation(room, entity);
                float radians = rotationMethod.Type == DataType.Number ? (float)rotationMethod.Number : (float)script.Call(rotationMethod, room.ToLuaTable(script), entity.ToLuaTable(script)).Number;
                return radians;
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while getting rotation for entity {Name}: \n {e.Formatted()}");
                return base.Rotation(room, entity);
            }
        }

        /// <inheritdoc/>
        public override int Depth(RoomData room, Entity entity)
        {
            try
            {
                DynValue depthMethod = entityTable.Get("depth");
                if (depthMethod.IsNil())
                    return base.Depth(room, entity);
                int depth = depthMethod.Type == DataType.Number ? (int)depthMethod.Number : (int)script.Call(depthMethod, room.ToLuaTable(script), entity.ToLuaTable(script)).Number;
                return depth;
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while getting depth for entity {Name}: \n {e.Formatted()}");
                return base.Depth(room, entity);
            }
        }

        /// <inheritdoc/>
        public override List<string> Mods()
        {
            List<string> mods = [modName];
            try
            {
                DynValue associatedMods = entityTable.Get("associatedMods");
                if (associatedMods.IsNil())
                    return mods;
                Table extra = associatedMods.Type == DataType.Table ? associatedMods.Table : script.Call(associatedMods, Entity.DefaultFromData(this, RoomData.Default)).Table;
                return [modName, .. extra.Values.Select(t => t.String)];
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while getting associated mods for entity {Name}: \n {e.Formatted()}");
                return mods;
            }
        }

        /// <inheritdoc/>
        public override List<Rectangle> Selection(RoomData room, Entity entity)
        {
            try
            {
                DynValue selectionMethod = entityTable.Get("selection");
                if (selectionMethod.IsNil())
                    return base.Selection(room, entity);

                DynValue result = script.Call(selectionMethod, room.ToLuaTable(script), entity.ToLuaTable(script));
                if (result.Type == DataType.Table)
                {
                    return [result.Table.ToRectangle()];
                }
                else if (result.Type == DataType.Tuple)
                {
                    Table main = result.Tuple[0].Table;
                    Table nodes = result.Tuple[1].Table;
                    List<Rectangle> output = [];

                    output.Add(main.ToRectangle());
                    if (nodes == null)
                        return output;

                    foreach (DynValue node in nodes.Values)
                    {
                        output.Add(node.Table.ToRectangle());
                    }
                }

                return base.Selection(room, entity);
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while getting selection for entity {Name}: \n {e.Formatted()}");
                return base.Selection(room, entity);
            }
        }

        /// <inheritdoc/>
        public override Rectangle Rectangle(RoomData room, Entity entity)
        {
            try
            {
                DynValue rectangleMethod = entityTable.Get("rectangle");
                if (rectangleMethod.IsNil())
                    return base.Rectangle(room, entity);

                DynValue result = script.Call(rectangleMethod, room.ToLuaTable(script), entity.ToLuaTable(script));
                return result.Table.ToRectangle();
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while getting rectangle for entity {Name}: \n {e.Formatted()}");
                return base.Rectangle(room, entity);
            }
        }

        /// <inheritdoc/>
        public override Rectangle NodeRectangle(RoomData room, Entity entity, int nodeIndex)
        {
            try
            {
                DynValue rectangleMethod = entityTable.Get("nodeRectangle");
                if (rectangleMethod.IsNil())
                    return base.Rectangle(room, entity);

                DynValue result = script.Call(rectangleMethod, room.ToLuaTable(script), entity.ToLuaTable(script), entity.GetNode(nodeIndex).ToLuaTable(script), nodeIndex);
                return result.Table.ToRectangle();
            }
            catch (Exception e)
            {
                Logger.Error("LuaEntity", $"Error while getting node rectangle for entity {Name}: \n {e.Formatted()}");
                return base.Rectangle(room, entity);
            }
        }

        /// <inheritdoc/>
        public override Rectangle GetDefaultRectangle(RoomData room, Entity entity, int nodeIndex)
        {
            if (nodeIndex == -1 && !entityTable.Get("rectangle").IsNil())
                return Rectangle(room, entity);
            if (nodeIndex >= 0 && !entityTable.Get("nodeRectangle").IsNil())
                return NodeRectangle(room, entity, nodeIndex);
            return base.GetDefaultRectangle(room, entity, nodeIndex);
        }
    }
}