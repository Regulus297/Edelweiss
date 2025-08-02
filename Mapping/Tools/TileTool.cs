using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Tools
{
    public abstract class TileTool : MappingTool
    {
        public override List<string> Layers => ["Foreground", "Background"];
        public override List<string> Materials => (selectedLayer == 0? MainPlugin.Instance.fgTiles: MainPlugin.Instance.bgTiles).Select(t => t.Value.name).ToList();
        public override List<string> MaterialIDs => (selectedLayer == 0? MainPlugin.Instance.fgTiles: MainPlugin.Instance.bgTiles).Select(t => t.Value.ID).ToList();

        protected void SetTile(ref string tileData, JObject room, int x, int y)
        {
            if (!TileInBounds(room, x, y))
                return;
                
            int i = y * ((int)room["width"] / 8) + x;
            tileData = tileData.Substring(0, i) + selectedMaterial + tileData.Substring(i + 1);
        }

        protected bool TileInBounds(JObject room, int x, int y)
        {
            return (x >= 0) && (x < ((int)room["width"] / 8)) && (y >= 0) && (y < ((int)room["height"] / 8));
        }
    }
}