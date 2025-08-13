using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Edelweiss.Loenn;
using Edelweiss.Utils;
using MoonSharp.Interpreter;

namespace Edelweiss.Mapping.Entities
{
    /// <summary>
    /// Class containing data for an entity placed in the map.
    /// </summary>
    public class Entity(string name, string id, params Point[] nodes) : ILuaConvertible
    {

        /// <summary>
        /// 
        /// </summary>
        public string _name = name;

        /// <summary>
        /// 
        /// </summary>
        public string _id = id;

        /// <summary>
        /// 
        /// </summary>
        public string _type = "entity";

        /// <summary>
        /// The positions of the nodes the entity has
        /// </summary>
        public List<Point> nodes = nodes.ToList();

        /// <summary>
        /// The placement data for the entity;
        /// </summary>
        public Dictionary<string, object> data = [];
        
        public int x = 0, y = 0, width = 0, height = 0;

        /// <summary>
        /// Creates a default entity with the placement data from the given entity data
        /// </summary>
        public static Entity DefaultFromData(EntityData entityData)
        {
            return new(entityData.Name, "")
            {
                data = entityData.GetPlacementData()
            };
        }

        /// <summary>
        /// Converts the entity to a Lua table compatible with Loenn
        /// </summary>
        public Table ToLuaTable(Script script)
        {
            Table table = new(script);
            table["_name"] = _name;
            table["_id"] = _id;
            table["_type"] = _type;

            table["x"] = x;
            table["y"] = y;
            table["width"] = width;
            table["height"] = height;

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