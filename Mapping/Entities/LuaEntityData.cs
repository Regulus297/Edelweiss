using System;
using System.Collections.Generic;
using System.Linq;
using Edelweiss.Mapping.Drawables;
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
        public override string Name => Language.GetTextOrDefault($"Loenn.entities.{name}.placements.name.{placementName}") ?? placementName;

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
                MainPlugin.Instance.Logger.Error($"Error while drawing entity {Name}: \n {e.Formatted()}");

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
                return script.Call(justificationMethod, room.ToLuaTable(script), entity.ToLuaTable(script)).Table.Values.Select(v => (float)v.Number).ToList(); ;
            }
            catch (Exception e)
            {
                MainPlugin.Instance.Logger.Error($"Error while getting justification for entity {Name}: \n {e.Formatted()}");
                return [0.5f, 0.5f];
            }
        }

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
                MainPlugin.Instance.Logger.Error($"Error while getting colour for entity {Name}: \n {e.Formatted()}");
                return base.Color(room, entity);
            }
        }

        public override string FillColor(RoomData room, Entity entity)
        {
            try
            {
                DynValue colorMethod = entityTable.Get("fillColor");
                if (colorMethod.IsNil())
                    return base.FillColor(room, entity);

                if (colorMethod.Type == DataType.Table)
                    return colorMethod.Color();
                return script.Call(colorMethod, room.ToLuaTable(script), entity.ToLuaTable(script)).Color();
            }
            catch (Exception e)
            {
                MainPlugin.Instance.Logger.Error($"Error while getting fill colour for entity {Name}: \n {e.Formatted()}");
                return base.FillColor(room, entity);
            }
        }

        public override string BorderColor(RoomData room, Entity entity)
        {
            try
            {
                DynValue colorMethod = entityTable.Get("borderColor");
                if (colorMethod.IsNil())
                    return base.BorderColor(room, entity);

                if (colorMethod.Type == DataType.Table)
                    return colorMethod.Color();
                return script.Call(colorMethod, room.ToLuaTable(script), entity.ToLuaTable(script)).Color();
            }
            catch (Exception e)
            {
                MainPlugin.Instance.Logger.Error($"Error while getting border colour for entity {Name}: \n {e.Formatted()}");
                return base.FillColor(room, entity);
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
                MainPlugin.Instance.Logger.Error($"Error while getting placement data for entity {Name}: \n {e.Formatted()}");
                return [];
            }
        }
    }
}