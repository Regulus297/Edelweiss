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
        internal Dictionary<string, TileData> bgTiles;
        internal SyncedVariable fgTileNames = new("Edelweiss:ForegroundTileNames");
        internal SyncedVariable fgTileMasks = new("Edelweiss:ForegroundTiles");
        internal SyncedVariable fgTileKeys = new("Edelweiss:ForegroundTileKeys");
        internal SyncedVariable bgTileNames = new("Edelweiss:BackgroundTileNames");
        internal SyncedVariable bgTileMasks = new("Edelweiss:BackgroundTiles");
        internal SyncedVariable bgTileKeys = new("Edelweiss:BackgroundTileKeys");
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
            LoadForegroundTiles();
            LoadBackgroundTiles();




            if (!CelesteModLoader.LoadTextures(Path.Join(CelesteDirectory, "graphics-dump")))
            {
                Console.WriteLine("Failed to load textures: please place graphics dump inside celeste directory");
            }
            textures.Value = CelesteModLoader.texturePaths;
        }

        private void LoadForegroundTiles()
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
        }

        private void LoadBackgroundTiles()
        {
            bgTiles = TileLoader.LoadTileXML(Path.Join(CelesteDirectory, "Content", "Graphics", "BackgroundTiles.xml"));
            bgTileNames.Value = bgTiles.Select(t => t.Value.name).ToList();

            Dictionary<string, Dictionary<string, List<Point>>> masks = [];
            Dictionary<string, string> keys = [];
            foreach (var tile in bgTiles)
            {
                masks[tile.Key] = tile.Value.masks;
                keys[tile.Key] = tile.Value.path;
            }

            bgTileMasks.Value = masks;
            bgTileKeys.Value = keys;
        }
    }
}