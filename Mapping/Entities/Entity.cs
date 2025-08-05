using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Edelweiss.Loenn;
using Edelweiss.Utils;
using MoonSharp.Interpreter;

namespace Edelweiss.Mapping.Entities
{
    public class Entity(string name, string id, params Point[] nodes): ILuaConvertible
    {
        public static Entity Default = new("", "");
        public string _name = name;
        public string _id = id;
        public string _type = "entity";
        public List<Point> nodes = nodes.ToList();
        public Dictionary<string, object> data = [];

        public static Entity DefaultFromData(EntityData entityData)
        {
            return new("", "")
            {
                data = entityData.GetPlacementData()
            };
        }

        public Table ToLuaTable(Script script)
        {
            Table table = new(script);
            table["_name"] = _name;
            table["_id"] = _id;
            table["_type"] = _type;

            // TODO: update this to actually reflect the entity's position
            table["x"] = 0;
            table["y"] = 0;
            table["width"] = 0;
            table["height"] = 0;

            Table nodesTable = new Table(script);
            foreach (Point p in nodes)
            {
                nodesTable.Append(DynValue.NewTable(script, DynValue.NewNumber(p.X), DynValue.NewNumber(p.Y)));
            }
            table["nodes"] = nodesTable;
            foreach (var d in data)
            {
                table[d.Key] = DynValue.FromObject(script, d.Value);
            }
            return table;
        }
    }
}