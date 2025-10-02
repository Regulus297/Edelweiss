using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Edelweiss.Loenn;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Tools;
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
        /// The name of the entity without the placement
        /// </summary>
        public string EntityName => _name.Split(".")[0];

        /// <summary>
        /// The name of the placement without the entity name
        /// </summary>
        public string PlacementName => _name.Split(".")[^1];

        /// <summary>
        /// The name of the entity. Setting it automatically changes the entity data
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = CelesteModLoader.defaultPlacements[value];
                entityData = CelesteModLoader.entities[_name];
            }
        }

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

        /// <summary>
        /// The rotation of the entity
        /// </summary>
        public float rotation => entityData.Rotation(entityRoom ?? RoomData.Default, this) * 180 / MathF.PI;

        /// <summary>
        /// The x-axis justification of the entity
        /// </summary>
        public float justificationX => entityData.Justification(entityRoom ?? RoomData.Default, this)[0];

        /// <summary>
        /// The y-axis justification of the entity
        /// </summary>
        public float justificationY => entityData.Justification(entityRoom ?? RoomData.Default, this)[1];

        /// <summary>
        /// The x-axis scale of the entity
        /// </summary>
        public float scaleX => entityData.Scale(entityRoom ?? RoomData.Default, this)[0];

        /// <summary>
        /// The y-axis scale of the entity
        /// </summary>
        public float scaleY => entityData.Scale(entityRoom ?? RoomData.Default, this)[1];

        /// <summary>
        /// The depth of the entity
        /// </summary>
        public int depth => entityData.Depth(entityRoom ?? RoomData.Default, this);

        internal Func<int, bool> customRotate;
        internal Func<bool, bool, bool> customFlip;
        internal Func<int, bool> customCycle;

        private int nodeOffsetX, nodeOffsetY;

        internal EntityData entityData;
        internal RoomData entityRoom;

        internal static Queue<string> DiscardedIDs = [];

        internal static string GetEntityID()
        {
            if (DiscardedIDs.TryDequeue(out string id))
            {
                return id;
            }
            return MappingTab.map.allEntities.Count.ToString();
        }

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
                created.width = width.GetType() == typeof(double) ? (int)(double)width : (int)width;

            if (created.data.TryGetValue("height", out object height))
                created.height = height.GetType() == typeof(double) ? (int)(double)height : (int)height;

            if (created.data.TryGetValue("nodes", out object nodes))
            {
                List<Point> nodesList = nodes as List<Point>;
                if (nodes is Table table)
                {
                    foreach (DynValue v in table.Values)
                    {
                        nodesList.Add(new Point((int)v.Table.Get<double>("x"), (int)v.Table.Get<double>("y")));
                    }
                }
                if (nodesList != null)
                {
                    created.nodes = nodesList;
                }
            }

            for (int i = created.nodes.Count; i < entityData.NodeLimits(room, created)[0]; i++)
            {
                created.nodes.Add(new Point((i + 1) * 32, 0));
            }

            return created;
        }

        /// <summary>
        /// Resizes the entity to the desired width and height while respecting the size bounds for the entity
        /// </summary>
        public void Resize(int width, int height, int step = 1)
        {
            List<int> bounds = entityData.SizeBounds(entityRoom ?? RoomData.Default, this);
            if (step <= 1)
            {
                width = Math.Clamp(width, bounds[0], bounds[2]);
                height = Math.Clamp(height, bounds[1], bounds[3]);
            }
            else
            {
                if (width < bounds[0])
                {
                    width += (1 + (bounds[0] - width) / step) * step;
                }
                else if (width > bounds[2])
                {
                    width -= (width - bounds[2]) / step * step;
                }

                if (height < bounds[1])
                {
                    height += (1 + (bounds[1] - height) / step) * step;
                }
                else if (height > bounds[3])
                {
                    height -= (height - bounds[3]) / step * step;
                }
            }
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
            Table meta = new(script);
            table.MetaTable = meta;

            meta["__index"] = (Table t, string key) =>
            {
                if (key == "_name")
                    return DynValue.NewString(_name);
                if (key == "depth")
                    return DynValue.NewNumber(depth);
                if (data.ContainsKey(key))
                    return DynValue.FromObject(script, data[key]);
                return DynValue.Nil;
            };

            meta["__newindex"] = (Table t, string key, DynValue value) =>
            {
                if (key == "_name")
                {
                    Name = value.String;
                }
                else if (key == "x")
                {
                    x = (int)value.Number;
                }
                else if (key == "y")
                {
                    y = (int)value.Number;
                }
                else if (key == "width")
                {
                    width = (int)value.Number;
                }
                else if (key == "height")
                {
                    height = (int)value.Number;
                }
                else if (key == "customCycle")
                {
                    customCycle = (int amount) =>
                    {
                        return script.Call(value.Callback, amount).Boolean;
                    };
                }
                else if (key == "customFlip")
                {
                    customFlip = (bool h, bool w) => script.Call(value.Callback, h, w).Boolean;
                }
                else if (key == "customRotate")
                {
                    customRotate = (int rotation) => script.Call(value.Callback, rotation).Boolean;
                }

                if (data.ContainsKey(key))
                {
                    data[key] = value.Value();
                }
            };

            table["_id"] = _id;
            table["_type"] = _type;


            table["x"] = x;
            table["y"] = y;
            table["width"] = width;
            table["height"] = height;

            Table nodesTable = new Table(script);
            Table nodeMeta = new Table(script);
            nodesTable.MetaTable = nodeMeta;

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
            return new Point(point.X + x + nodeOffsetX, point.Y + y + nodeOffsetY).ToLuaTable(script);
        }

        /// <summary>
        /// Gets the position of the node in global space
        /// </summary>
        /// <param name="nodeIndex">The node's 0-based index</param>
        public Point GetNode(int nodeIndex)
        {
            if (nodeIndex >= nodes.Count)
                return Point.Empty;

            return new Point(nodes[nodeIndex].X + x + nodeOffsetX, nodes[nodeIndex].Y + y + nodeOffsetY);
        }

        /// <summary>
        /// Sets the value nodes should be offset by. Used when drawing sprite for nodes when no nodeSprite is defined
        /// </summary>
        public void SetNodeOffset(int x, int y)
        {
            nodeOffsetX = x;
            nodeOffsetY = y;
        }

        /// <summary>
        /// Draws the entity to the frontend
        /// </summary>
        /// <param name="entityObject">The ID of the entity object in the viewport if the entity should be drawn to an existing object. Null if not.</param>
        /// <param name="entityIndex"></param>
        /// <param name="updatePosition"></param>
        public void Draw(string entityObject = null, int entityIndex = 0, bool updatePosition = false)
        {
            JObject item = new()
            {
                {"name", _id},
                {"x", x},
                {"y", y},
                {"width", width},
                {"height", height},
                {"rotation", entityData.Rotation(entityRoom ?? RoomData.Default, this)},
                {"onSelectionMoved", SelectionTool.SelectionMovedNetcode},
                {"onSelectionChanged", SelectionTool.SelectionChangedNetcode},
                {"onSelectionResized", SelectionTool.SelectionResizedNetcode},
                { "selectable", "@defer('@getVar(\\'Edelweiss:SelectionActive\\')')"}
            };

            JArray selection = [];
            int j = 0;
            foreach (Rectangle rectangle in entityData.Selection(entityRoom ?? RoomData.Default, this))
            {
                List<bool> canResize = entityData.CanResize(entityRoom ?? RoomData.Default, this);
                selection.Add(new JObject()
                {
                    {"x", rectangle.X},
                    {"y", rectangle.Y},
                    {"width", rectangle.Width},
                    {"height", rectangle.Height},
                    {"resizeX", canResize[0] && j == 0},
                    {"resizeY", canResize[1] && j == 0},
                    {"tags", new JArray() {
                        {EntityName}
                    }}
                });
                j++;
            }

            item["selection"] = selection;


            JArray shapes = new();
            entityData.Draw(shapes, entityRoom ?? RoomData.Default, this);

            item["shapes"] = shapes;

            if (entityData.NodeVisibility(this) != Visibility.Never)
            {
                int i = 0;
                NodeLineRenderType nodeLineRenderType = entityData.NodeLineRenderType(this);
                foreach (Point point in nodes)
                {
                    if (nodeLineRenderType == NodeLineRenderType.Fan)
                    {
                        shapes.Add(new JObject() {
                        {"type", "line"},
                        {"x1", 0},
                        {"y1", 0},
                        {"x2", point.X},
                        {"y2", point.Y},
                        {"color", "#ffffff"},
                        {"thickness", LoveModule.PEN_THICKNESS},
                        {"depth", depth + 1}
                    });
                    }
                    else if (nodeLineRenderType == NodeLineRenderType.Line)
                    {
                        Point previous = i == 0 ? Point.Empty : nodes[i - 1];
                        shapes.Add(new JObject() {
                        {"type", "line"},
                        {"x1", previous.X},
                        {"y1", previous.Y},
                        {"x2", point.X},
                        {"y2", point.Y},
                        {"color", "#ffffff"},
                        {"thickness", LoveModule.PEN_THICKNESS},
                        {"depth", depth + 1}
                    });
                    }
                    else if (nodeLineRenderType == NodeLineRenderType.Circle)
                    {
                        shapes.Add(new JObject() {
                        {"type", "circle"},
                        {"radius", point.Distance(new Point(0, 0))},
                        {"x", point.X},
                        {"y", point.Y},
                        {"color", "#ffffff"},
                        {"thickness", LoveModule.PEN_THICKNESS},
                        {"depth", depth + 1}
                    });
                    }
                    i++;


                    using (new SpriteDestination(null, 0, 0))
                    {
                        entityData.NodeDraw(shapes, entityRoom, this, i - 1);
                    }
                }
            }

            if (entityObject == null)
            {
                NetworkManager.SendPacket(Netcode.ADD_ITEM, new JObject()
                {
                    { "widget", "Mapping/MainView" },
                    { "parent", entityRoom.name },
                    { "item", item }
                });
            }
            else
            {
                NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
                {
                    { "widget", "Mapping/MainView" },
                    { "item", entityObject },
                    { "index", entityIndex },
                    { "action", "modify" },
                    {
                        "data",
                        new JObject()
                        {
                            { "shapes", shapes },
                            { "rotation", entityData.Rotation(entityRoom ?? RoomData.Default, this) }
                        }
                    }
                });
                if (updatePosition)
                {
                    NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
                    {
                        { "widget", "Mapping/MainView" },
                        { "item", entityObject },
                        {
                            "data",
                            new JObject()
                            {
                                { "x", x },
                                { "y", y },
                                { "width", width },
                                { "height", height },
                                { "selection", selection }
                            }
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Rotates the entity by a given direction. Clockwise rotations are positive and anticlockwise ones are negative
        /// </summary>
        /// <param name="direction"></param>
        public bool Rotate(int direction)
        {
            return customRotate?.Invoke(direction) ?? entityData.Rotate(entityRoom ?? RoomData.Default, this, direction);
        }

        /// <summary>
        /// Flips the entity along the given axes
        /// </summary>
        /// <param name="horizontal">Whether or not the entity should be flipped horizontally</param>
        /// <param name="vertical">Whether or not the entity should be flipped vertically</param>
        /// <returns>True if flipping affected the entity, false if not</returns>
        public bool Flip(bool horizontal, bool vertical)
        {
            return customFlip?.Invoke(horizontal, vertical) ?? entityData.Flip(entityRoom ?? RoomData.Default, this, horizontal, vertical);
        }

        /// <summary>
        /// Cycles the entity's state by the given amount
        /// </summary>
        public bool Cycle(int amount)
        {
            return customCycle?.Invoke(amount) ?? entityData.Cycle(entityRoom ?? RoomData.Default, this, amount);
        }

        /// <summary>
        /// Adds a node to the entity if nodelimits allow
        /// </summary>
        /// <param name="relativePosition">The position of the node relative to the entity</param>
        public void AddNode(Point relativePosition)
        {
            int limit = entityData.NodeLimits(entityRoom ?? RoomData.Default, this)[1];
            if (limit == -1 || nodes.Count < limit)
            {
                nodes.Add(relativePosition);
            }
        }

        /// <summary>
        /// Attempts to remove a node from the entity respecting the node limits
        /// </summary>
        /// <param name="index">The index of the node to remove</param>
        /// <returns>Whether the node could successfully be removed</returns>
        public bool TryRemoveNode(int index)
        {
            int limit = entityData.NodeLimits(entityRoom ?? RoomData.Default, this)[0];
            if (nodes.Count == limit)
            {
                return false;
            }
            nodes.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Gets the data with the given key
        /// </summary>
        public object this[string key]
        {
            get
            {
                return data.GetValueOrDefault(key);
            }
            set
            {
                data[key] = value;
            }
        }
    }
}