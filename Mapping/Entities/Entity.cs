using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Edelweiss.Loenn;
using Edelweiss.Network;
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
        EntityData entityData;
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
            string entityName;
            if (entityObject == null)
            {
                NetworkManager.SendPacket(Netcode.ADD_ITEM, new JObject()
                {
                    {"widget", "Mapping/MainView"},
                    {"parent", entityRoom.name},
                    {"item", item}
                });
                entityObject = $"{entityRoom.name}/{_id}";
                entityName = _id;
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
                entityName = entityObject;
            }

            int i = 0;
            string nodeLineRenderType = entityData.NodeLineRenderType(this);
            foreach (Point point in nodes)
            {
                JArray nodeShapes = new()
                {
                };

                if (nodeLineRenderType == "fan")
                {
                    nodeShapes.Add(new JObject() {
                        {"type", "line"},
                        {"x1", 0},
                        {"y1", 0},
                        {"x2", $"@defer('@itemX(\\'Mapping/MainView\\', \\'{entityObject}\\', \\'{entityObject}/{entityName}_node{i}\\')')"},
                        {"y2", $"@defer('@itemY(\\'Mapping/MainView\\', \\'{entityObject}\\', \\'{entityObject}/{entityName}_node{i}\\')')"},
                        {"color", "#ffffff"},
                        {"thickness", LoveModule.PEN_THICKNESS}
                    });
                }
                else if (nodeLineRenderType == "line")
                {
                    string previous = i == 0 ? entityObject : $"{entityObject}/{entityObject}_node{i - 1}";
                    nodeShapes.Add(new JObject() {
                        {"type", "line"},
                        {"x1", 0},
                        {"y1", 0},
                        {"x2", $"@defer('@itemX(\\'Mapping/MainView\\', \\'{previous}\\', \\'{entityObject}/{entityName}_node{i}\\')')"},
                        {"y2", $"@defer('@itemY(\\'Mapping/MainView\\', \\'{previous}\\', \\'{entityObject}/{entityName}_node{i}\\')')"},
                        {"color", "#ffffff"},
                        {"thickness", LoveModule.PEN_THICKNESS}
                    });
                }
                else if (nodeLineRenderType == "circle")
                {
                    nodeShapes.Add(new JObject() {
                        {"type", "circle"},
                        {"radius", point.Distance(new Point(0, 0))},
                        { "x", $"@defer('@itemX(\\'Mapping/MainView\\', \\'{entityObject}\\', \\'{entityObject}/{entityName}_node{i}\\')')"},
                        {"y", $"@defer('@itemY(\\'Mapping/MainView\\', \\'{entityObject}\\', \\'{entityObject}/{entityName}_node{i}\\')')"},
                        {"color", "#ffffff"},
                        {"thickness", LoveModule.PEN_THICKNESS}
                    });
                }


                JObject node = new()
                {
                    {"x", point.X},
                    {"y", point.Y},
                    {"width", 8},
                    {"height", 8},
                    { "name", $"{entityName}_node{i}"},
                    { "shapes", nodeShapes },
                    {"opacity", opacity}
                };
                i++;

                int prevX = x;
                int prevY = y;
                x = point.X;
                y = point.Y;

                entityData.Draw(nodeShapes, entityRoom, this);

                x = prevX;
                y = prevY;

                NetworkManager.SendPacket(Netcode.ADD_ITEM, new JObject()
                {
                    {"widget", "Mapping/MainView"},
                    {"item", node},
                    {"parent", entityObject}
                });
            }
        }
    }
}