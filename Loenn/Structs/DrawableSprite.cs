using System;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities;
using Edelweiss.Plugins;
using Edelweiss.Utils;
using MoonSharp.Interpreter;

namespace Edelweiss.Loenn.Structs
{
    internal class DrawableSprite : LoennModule
    {
        public override string ModuleName => "structs.drawable_sprite";

        public override Table GenerateTable(Script script)
        {
            Table table = new(script);

            var fromTexture = (Func<string, Table, DynValue>)((texture, data) =>
            {
                TextureData textureData;
                if ((textureData = CelesteModLoader.GetTextureData("Gameplay/" + texture)) == null)
                {
                    return DynValue.Nil;
                }
                return DynValue.NewTable(new Sprite(texture)
                {
                    x = (int)(data?.Get("x").Number ?? 0),
                    y = (int)(data?.Get("y").Number ?? 0),     
                }.ToLuaTable(script));
            });
            table["fromTexture"] = fromTexture;

            table["fromInternalTexture"] = (string texture, Table data) => fromTexture($"@Internal@/{texture}", data);

            return table;
        }
    }
}