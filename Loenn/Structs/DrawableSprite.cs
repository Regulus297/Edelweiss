using System;
using Edelweiss.Mapping.Entities;
using MoonSharp.Interpreter;

namespace Edelweiss.Loenn.Structs
{
    internal class DrawableSprite : LoennModule
    {
        public override string ModuleName => "structs.drawable_sprite";

        public override Table GenerateTable(Script script)
        {
            Table table = new(script);

            table["fromTexture"] = (Func<string, Table, DynValue>)((texture, data) =>
            {
                return DynValue.NewTable(new Sprite(texture).ToLuaTable(script));
            });

            return table;
        }
    }
}