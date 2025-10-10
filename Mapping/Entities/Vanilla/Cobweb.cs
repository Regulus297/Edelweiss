using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Loenn;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Utils;
using MoonSharp.Interpreter;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Cobweb : CSEntityData
    {
        public override string EntityName => "cobweb";

        public override List<string> PlacementNames()
        {
            return ["cobweb"];
        }

        public override List<int> NodeLimits(RoomData room, Entity entity) => [1, -1];
        public override Visibility NodeVisibility(Entity entity) => Visibility.Never;
        public override int Depth(RoomData room, Entity entity) => -1;

        private Line FromMiddle(Point middle, Point target, string color)
        {
            Point control = new Point((middle.X + target.X) / 2, 4 + (middle.Y + target.Y) / 2);
            Script script = new();
            Table drawing = LoennModule.RequireModule(script, "utils.drawing").Table;
            Table points = script.Call(drawing.Get("getSimpleCurve"), target.ToLuaTable(script, false), middle.ToLuaTable(script, false), control.ToLuaTable(script, false)).Table;

            Table drawableLine = LoennModule.RequireModule(script, "structs.drawable_line").Table;
            Table lineTable = script.Call(drawableLine.Get("fromPoints"), points, color).Table;
            return new Line(lineTable);
        }

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            string color = entity.Get("color", "#ffffff");
            Point origin = new(entity.x, entity.y);
            Point firstNode = entity.nodes[0];
            firstNode.X += entity.x;
            firstNode.Y += entity.y;

            Script script = new();
            Table drawing = LoennModule.RequireModule(script, "utils.drawing").Table;
            Point control = new Point((origin.X + firstNode.X) / 2, 4 + (origin.Y + firstNode.Y) / 2);
            Table middleTable = script.Call(drawing.Get("getCurvePoint"), origin.ToLuaTable(script, false), firstNode.ToLuaTable(script, false), control.ToLuaTable(script, false), 0.5f).Table;
            Point middle = new Point((int)(double)middleTable[1], (int)(double)middleTable[2]);

            List<Drawable> lines = [];

            foreach (Point node in entity.nodes)
            {
                lines.Add(FromMiddle(middle, new Point(node.X + entity.x, node.Y + entity.y), color));
            }

            lines.Add(FromMiddle(middle, origin, color));

            return lines;
        }

        public override Dictionary<string, object> GetPlacementData()
        {
            return new Dictionary<string, object>()
            {
                {"color", "#696A6A"}
            };
        }

        public override Rectangle GetDefaultRectangle(RoomData room, Entity entity, int nodeIndex)
        {
            Point pos = nodeIndex == -1 ? new Point(entity.x, entity.y) : entity.GetNode(nodeIndex);
            return new Rectangle(pos.X - 2, pos.Y - 2, 5, 5);
        }
    }
}