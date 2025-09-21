using System;
using System.Collections.Generic;
using System.IO;
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
        internal SyncedVariable fgTileData = new("Edelweiss:ForegroundTiles");
        internal SyncedVariable bgTileData = new("Edelweiss:BackgroundTiles");
        internal static SyncedVariable textures = new("Edelweiss:Textures");

        internal static string CelesteDirectory => Registry.registry[typeof(PluginSaveablePreference)].GetValue<CelesteDirectoryPref>().Value.ToString();

        public override void Load()
        {
            Instance = this;
        }

        public override void PostSetupContent()
        {
            AtlasLoader.LoadAtlasMetaFile(Path.Join(CelesteDirectory, "Content/Graphics/Atlases/Gameplay.meta"));
            AtlasLoader.LoadAtlasMetaFile(Path.Join(CelesteDirectory, "Content/Graphics/Atlases/Gui.meta"));
            LoadForegroundTiles();
            LoadBackgroundTiles();

            // if (!CelesteModLoader.LoadTexturesFromDirectory(Path.Join(CelesteDirectory, "graphics-dump")))
            // {
            //     Console.WriteLine("Failed to load textures: please place graphics dump inside celeste directory");
            // }
            textures.Value = CelesteModLoader.texturePaths;
        }

        private void LoadForegroundTiles()
        {
            fgTiles = TileLoader.LoadTileXML(Path.Join(CelesteDirectory, "Content", "Graphics", "ForegroundTiles.xml"));
            fgTileData.Value = fgTiles;
        }

        private void LoadBackgroundTiles()
        {
            bgTiles = TileLoader.LoadTileXML(Path.Join(CelesteDirectory, "Content", "Graphics", "BackgroundTiles.xml"));
            bgTileData.Value = bgTiles;
        }
    }
}