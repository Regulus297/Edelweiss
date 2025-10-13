using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Loenn;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities.Helpers;
using Edelweiss.Utils;
using MoonSharp.Interpreter;

namespace Edelweiss.Mapping.Entities.Vanilla
{
    internal class Glider : CSEntityData, IFieldInfoEntity
    {
        public override string EntityName => "glider";

        public override List<string> PlacementNames()
        {
            return ["normal", "floating"];
        }

        public override int Depth(RoomData room, Entity entity) => -5;

        public override List<Drawable> Sprite(RoomData room, Entity entity)
        {
            Sprite glider = new Sprite("objects/glider/idle0", entity);
            if (entity.Get<bool>("bubble"))
            {
                Script script = new Script();

                Table drawing = LoennModule.RequireModule(script, "utils.drawing").Table;
                Table points = script.Call(
                    drawing.Get("getSimpleCurve"),
                    new Point(entity.x - 11, entity.y - 1).ToLuaTable(script, false),
                    new Point(entity.x + 11, entity.y - 1).ToLuaTable(script, false),
                    new Point(entity.x, entity.y - 6).ToLuaTable(script, false)
                ).Table;

                Table drawableLine = LoennModule.RequireModule(script, "structs.drawable_line").Table;
                Table lineTable = script.Call(drawableLine.Get("fromPoints"), points).Table;
                Line line = new Line(lineTable);
                return [glider, line];
            }
            return [glider];
        }

        public void InitializeFieldInfo(EntityFieldInfo fieldInfo)
        {
            fieldInfo.AddField("tutorial", false)
                .AddField("bubble", placement == "floating")
                .SetCyclableField("bubble");
        }
    }
}