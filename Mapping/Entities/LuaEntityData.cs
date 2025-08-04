using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;

namespace Edelweiss.Mapping.Entities
{
    public class LuaEntityData(Script script, Table entityTable) : EntityData
    {
        Script script = script;
        Table entityTable = entityTable;
        public override string Name => (string)entityTable["name"];
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

        public override List<float> Justification(RoomData room, Entity entity)
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
            return script.Call(justificationMethod, room.ToLuaTable(script), entity.ToLuaTable(script)).Table.Values.Select(v => (float)v.Number).ToList();;
        }
    }
}