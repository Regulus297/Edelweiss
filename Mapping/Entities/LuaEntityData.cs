using System;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;

namespace Edelweiss.Mapping.Entities
{
    public class LuaEntityData(string name, Table placement, Script script, Table entityTable) : EntityData
    {
        Script script = script;
        Table entityTable = entityTable;
        Table placement = placement;
        string name = name;
        public override string Name => name;
        public override string Texture(RoomData room, Entity entity)
        {
            try
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
            catch (Exception e)
            {
                MainPlugin.Instance.Logger.Error($"Error while getting texture for entity {Name}: \n {e}");
                return "";
            }
        }

        public override List<Sprite> Sprite(RoomData room, Entity entity)
        {
            try
            {
                DynValue spriteMethod = entityTable.Get("sprite");
                if (spriteMethod.IsNil())
                    return base.Sprite(room, entity);
                DynValue sprite = script.Call(spriteMethod, room.ToLuaTable(script), entity.ToLuaTable(script));
                if (sprite.Table.Get("texture").IsNil())
                {
                    List<Sprite> output = [];
                    // It's a list
                    foreach (var table in sprite.Table.Values)
                        output.Add(new Sprite(table.Table));
                    return output;
                }
                // It's one sprite
                return [new Sprite(sprite.Table)];
            }
            catch (Exception e)
            {
                MainPlugin.Instance.Logger.Error($"Error while getting sprite for entity {Name}: \n {e}");
                return [];
            }
        }


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
                MainPlugin.Instance.Logger.Error($"Error while getting justification for entity {Name}: \n {e}");
                return [0.5f, 0.5f];
            }
        }

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
                MainPlugin.Instance.Logger.Error($"Error while getting placement data for entity {Name}: \n {e}");
                return [];
            }
        }
    }
}