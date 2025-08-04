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

        public Table ToLuaTable(Script script)
        {
            Table table = new(script);
            table["_name"] = _name;
            table["_id"] = _id;
            table["_type"] = _type;
            Table nodesTable = new Table(script);
            foreach (Point p in nodes)
            {
                nodesTable.Append(DynValue.NewTable(script, DynValue.NewNumber(p.X), DynValue.NewNumber(p.Y)));
            }
            table["nodes"] = nodesTable;
            return table;
        }
    }
}