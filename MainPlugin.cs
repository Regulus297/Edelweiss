using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Edelweiss.Preferences;
using Edelweiss.RegistryTypes;
using Edelweiss.Utils;

namespace Edelweiss
{
    internal sealed class MainPlugin : Plugin
    {
        internal static MainPlugin Instance { get; private set; }
        public override string ID => "Edelweiss";
        internal Dictionary<string, TileData> fgTiles;
        internal SyncedVariable fgTileNames = new("Edelweiss:ForegroundTileNames");
        internal SyncedVariable fgTileMasks = new("Edelweiss:ForegroundTiles");
        internal SyncedVariable fgTileKeys = new("Edelweiss:ForegroundTileKeys");
        internal static SyncedVariable textures = new("Edelweiss:Textures");

        internal static string CelesteDirectory => Registry.registry[typeof(PluginSaveablePreference)].GetValue<CelesteDirectoryPref>().Value.ToString();

        public long NetcodeDynamic { get; private set; }

        public override void Load()
        {
            Instance = this;
            NetcodeDynamic = CreateNetcode(nameof(NetcodeDynamic), false);
        }

        public override void PostSetupContent()
        {
            fgTiles = TileLoader.LoadTileXML(Path.Join(CelesteDirectory, "Content", "Graphics", "ForegroundTiles.xml"));
            fgTileNames.Value = fgTiles.Select(t => t.Value.name).ToList();

            Dictionary<string, Dictionary<string, List<Point>>> masks = [];
            Dictionary<string, string> keys = [];
            foreach (var tile in fgTiles)
            {
                masks[tile.Key] = tile.Value.masks;
                keys[tile.Key] = tile.Value.path;
            }

            fgTileMasks.Value = masks;
            fgTileKeys.Value = keys;

            
            if (!CelesteModLoader.LoadTextures(Path.Join(CelesteDirectory, "graphics-dump")))
            {
                Console.WriteLine("Failed to load textures: please place graphics dump inside celeste directory");
            }
            textures.Value = CelesteModLoader.texturePaths;
        }
    }
}