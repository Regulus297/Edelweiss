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
        public override string ID => "Edelweiss";
        internal Dictionary<string, TileData> fgTiles;
        internal SyncedVariable fgTileNames = new("Edelweiss:ForegroundTiles");

        public long NetcodeDynamic { get; private set; }

        public override void Load()
        {
            NetcodeDynamic = CreateNetcode(nameof(NetcodeDynamic), false);
        }

        public override void PostSetupContent()
        {
            fgTiles = TileLoader.LoadTileXML(Path.Join(Registry.registry[typeof(PluginSaveablePreference)].GetValue<CelesteDirectoryPref>().Value.ToString(), "Content", "Graphics", "ForegroundTiles.xml"));
            fgTileNames.Value = fgTiles.Select(t => t.Value.name).ToList();
        }
    }
}