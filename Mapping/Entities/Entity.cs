using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Edelweiss.Loenn;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Edelweiss.Utils;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

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

        /// <summary>
        /// 
        /// </summary>
        public int x = 0, y = 0, width = 0, height = 0;

        internal EntityData entityData;
        RoomData entityRoom;

        /// <summary>
        /// Creates a default entity with the placement data from the given entity data
        /// </summary>
        public static Entity DefaultFromData(EntityData entityData, RoomData room)
        {
            Entity created = new(entityData.Name, "")
            {
                data = entityData.GetPlacementData()
            };

            created.entityData = entityData;
            created.entityRoom = room;

            if (created.data.TryGetValue("width", out object width))
                created.width = (int)(double)width;

            if (created.data.TryGetValue("height", out object height))
                created.height = (int)(double)height;

            for (int i = 0; i < entityData.NodeLimits(room, created)[0]; i++)
            {
                created.nodes.Add(new Point((i+1) * 32, 0));
            }

            return created;
        }

        public void Resize(int width, int height)
        {
            List<int> bounds = entityData.SizeBounds(entityRoom ?? RoomData.Default, this);
            width = Math.Clamp(width, bounds[0], bounds[2]);
            height = Math.Clamp(height, bounds[1], bounds[3]);
            this.width = width;
            this.height = height;
            data["width"] = width;
            data["height"] = height;
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
            foreach (var d in data)
            {
                table[d.Key] = DynValue.FromObject(script, d.Value);
            }

            table["x"] = x;
            table["y"] = y;
            table["width"] = width;
            table["height"] = height;

            Table nodesTable = new Table(script);
            foreach (Point p in nodes)
            {
                nodesTable.Append(DynValue.NewTable(PointToTable(p, script)));
            }
            table["nodes"] = nodesTable;
            return table;
        }
        
        /// <summary>
        /// Converts a point in local space to global space and then converts it to a table
        /// </summary>
        /// <param name="point"></param>
        /// <param name="script">The script the table should belong to</param>
        public Table PointToTable(Point point, Script script)
        {
            return new Point(point.X + x, point.Y + y).ToLuaTable(script);
        }

        /// <summary>
        /// Draws the entity to the frontend
        /// </summary>
        /// <param name="opacity"></param>
        /// <param name="entityObject">The ID of the entity object in the viewport if the entity should be drawn to an existing object. Null if not.</param>
        /// <param name="entityIndex"></param>
        public void Draw(float opacity = 1, string entityObject = null, int entityIndex = 0)
        {
            JObject item = new()
            {
                {"name", _id},
                {"x", x},
                {"y", y},
                {"width", width},
                {"height", height}
            };


            JArray shapes = new();
            entityData.Draw(shapes, entityRoom ?? RoomData.Default, this);

            item["shapes"] = shapes;
            if (entityObject == null)
            {
                NetworkManager.SendPacket(Netcode.ADD_ITEM, new JObject()
                {
                    {"widget", "Mapping/MainView"},
                    {"parent", entityRoom.name},
                    {"item", item}
                });
                entityObject = $"{entityRoom.name}/{_id}";
            }
            else
            {
                NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
                {
                    {"widget", "Mapping/MainView"},
                    {"item", entityObject},
                    {"index", entityIndex},
                    {"action", "add"},
                    {"shapes", shapes}
                });
            }

            if (entityData.NodeVisibility(this) == Visibility.Never)
                return;

            int i = 0;
            NodeLineRenderType nodeLineRenderType = entityData.NodeLineRenderType(this);
            foreach (Point point in nodes)
            {
                JArray nodeShapes = new()
                {
                };

                if (nodeLineRenderType == NodeLineRenderType.Fan)
                {
                    nodeShapes.Add(new JObject() {
                        {"type", "line"},
                        {"x1", 0},
                        {"y1", 0},
                        {"x2", point.X},
                        {"y2", point.Y},
                        {"color", "#ffffff"},
                        {"thickness", LoveModule.PEN_THICKNESS}
                    });
                }
                else if (nodeLineRenderType == NodeLineRenderType.Line)
                {
                    string previous = i == 0 ? entityObject : $"{entityObject}/{entityObject}_node{i - 1}";
                    nodeShapes.Add(new JObject() {
                        {"type", "line"},
                        {"x1", 0},
                        {"y1", 0},
                        {"x2", point.X},
                        {"y2", point.Y},
                        {"color", "#ffffff"},
                        {"thickness", LoveModule.PEN_THICKNESS}
                    });
                }
                else if (nodeLineRenderType == NodeLineRenderType.Circle)
                {
                    nodeShapes.Add(new JObject() {
                        {"type", "circle"},
                        {"radius", point.Distance(new Point(0, 0))},
                        {"x", point.X},
                        {"y", point.Y},
                        {"color", "#ffffff"},
                        {"thickness", LoveModule.PEN_THICKNESS}
                    });
                }
                i++;


                using (new SpriteDestination(null, -point.X, -point.Y))
                {
                    entityData.NodeDraw(nodeShapes, entityRoom, this, i - 1);
                }

                NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
                {
                    {"widget", "Mapping/MainView"},
                    {"item", entityObject},
                    {"action", "add"},
                    {"shapes", nodeShapes}
                });
            }
        }
    }
}