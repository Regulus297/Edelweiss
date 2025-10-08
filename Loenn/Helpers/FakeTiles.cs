using System;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Entities;
using Edelweiss.Mapping.Entities.Helpers;
using Edelweiss.Utils;
using MoonSharp.Interpreter;

namespace Edelweiss.Loenn.Helpers
{
    internal class FakeTiles : LoennModule
    {
        public override string ModuleName => "helpers.fake_tiles";

        public override Table GenerateTable(Script script)
        {
            Table fakeTilesHelper = new(script);

            fakeTilesHelper["getPlacementMaterial"] = (string fallback, DynValue layer, DynValue allowAir) =>
            {
                return TileHelper.GetMaterial(fallback, layer.String != "tilesBg", allowAir.IsNil() ? false : allowAir.Boolean);
            };

            fakeTilesHelper["getEntitySpriteFunction"] = (string materialKey, DynValue blendKey, DynValue layer, DynValue color, DynValue x, DynValue y) =>
            {
                bool foreground = layer.String != "tilesBg";
                (_, _, _, double opacity) = ((double, double, double, double))script.NewColor(color.Color()).Table.Unpack();
                return (Table room, Table entity, DynValue node) =>
                {
                    // Set the entity's customCycle here because there's literally nowhere else to do it
                    entity.SetToMetatable("customCycle", (int amount) =>
                    {
                        string id = entity.GetFromMetatable(materialKey).String;
                        entity.SetToMetatable(materialKey, TileHelper.GetCycleValue(id, amount, foreground));
                        return true;
                    });
                    
                    string id = entity.GetFromMetatable(materialKey).String;
                    return new Tiles(id, foreground, (int)(double)entity["x"], (int)(double)entity["y"], (int)(double)entity["width"] / 8, (int)(double)entity["height"] / 8)
                    {
                        depth = (int)entity.GetFromMetatable("depth").Number
                    }.ToLuaTable(script);
                };
            };

            fakeTilesHelper["getFieldInformation"] = (string materialKey, DynValue layer) =>
            {
                return DynValue.NewTable(script);
            };

            return fakeTilesHelper;
        }
    }
}