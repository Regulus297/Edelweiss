using System;
using Edelweiss.Mapping.Entities;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Loenn
{
    internal class LoveModule : LoennModule
    {
        public const float PEN_THICKNESS = 0.5f;


        public override string ModuleName => "love";
        public override bool Global => true;

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
                    {"color", "#ffffff"},
                    {"thickness", PEN_THICKNESS}
                });
            });
        }
    }
}