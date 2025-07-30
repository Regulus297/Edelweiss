using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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

        public long NetcodeDynamic { get; private set; }

        public override void Load()
        {
            NetcodeDynamic = CreateNetcode(nameof(NetcodeDynamic), false);
            CreateVar("ForegroundTiles", () => fgTiles.Select(i => i.Value.name).ToList());
        }

        public override void PostSetupContent()
        {
            fgTiles = TileLoader.LoadTileXML(Path.Join(Registry.registry[typeof(PluginSaveablePreference)].GetValue<CelesteDirectoryPref>().Value.ToString(), "Content", "Graphics", "ForegroundTiles.xml"));
        }
    }
}