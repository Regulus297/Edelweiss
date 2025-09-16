using System;
using Edelweiss.Loenn.Structs;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities;
using Edelweiss.Utils;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Loenn
{
    internal class LoveModule : LoennModule
    {
        public const float PEN_THICKNESS = 0.5f;


        public override string ModuleName => "love";
        public override bool Global => true;

        public static string color = "#ffffffff";

        public override Table GenerateTable(Script script)
        {
            Table love = new(script);

            InitGraphics(love, script);

            return love;
        }

        internal void InitGraphics(Table love, Script script)
        {
            Table graphics = new Table(script);
            love["graphics"] = graphics;

            graphics["setColor"] = (DynValue r, DynValue g, DynValue b, DynValue a) =>
            {
                if (r.Type != DataType.Table)
                {
                    r = DynValue.NewTable(script, r, g, b, a);
                }
                color = r.Color();
            };

            graphics["circle"] = (Action<string, double, double, double>)((mode, x, y, radius) =>
            {
                if (SpriteDestination.destination == null)
                    return;

                SpriteDestination.destination.Add(new JObject()
                {
                    {"type", "circle"},
                    {"x", (int)x - SpriteDestination.offsetX},
                    {"y", (int)y - SpriteDestination.offsetY},
                    {"radius", radius},
                    {"color", color},
                    {"thickness", PEN_THICKNESS}
                });
            });

            graphics["rectangle"] = (DynValue mode, DynValue x, DynValue y, DynValue width, DynValue height) =>
            {
                if (SpriteDestination.destination == null)
                    return;

                var module = RequireModule(script, "structs.drawable_rectangle").Table;
                Table table = script.Call(module.Get("fromRectangle"), mode, x, y, width, height, color, color).Table;
                Rectangle rectangle = new(table);
                rectangle.Draw();
            };
        }
    }
}