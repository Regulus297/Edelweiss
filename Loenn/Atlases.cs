using System;
using System.Collections.Generic;
using Edelweiss.Utils;
using MoonSharp.Interpreter;

namespace Edelweiss.Loenn
{
    internal class Atlases : LoennModule
    {
        public override string ModuleName => "atlases";


        public override Table GenerateTable(Script script)
        {
            Table table = new(script);
            Table meta = new(script);
            table.MetaTable = meta;

            table["getResource"] = (string tex, string atlasKey) =>
            {
                DynValue atlas = script.Call(meta["__index"], table, atlasKey.ToLower());
                return script.Call(atlas.Table.MetaTable["__index"], atlas.Table, tex);
            };

            meta["__index"] = (Func<Table, string, DynValue>)((t, key) =>
            {
                if (key == "gameplay")
                {
                    Table gameplay = new(script);
                    Table gameplayMt = new(script);
                    gameplay.MetaTable = gameplayMt;

                    gameplayMt["__index"] = (Func<Table, string, DynValue>)((t1, key1) =>
                    {
                        Table data = CelesteModLoader.GetTextureData("Gameplay/" + key1)?.ToLuaTable(script);
                        return data != null ? DynValue.NewTable(data) : DynValue.Nil;
                    });

                    return DynValue.NewTable(gameplay);
                }
                return DynValue.Nil;
            });

            return table;
        }
    }
}