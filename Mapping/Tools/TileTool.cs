using System.Collections.Generic;
using System.Linq;
using Edelweiss.Preferences;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Tools
{
    public abstract class TileTool : MappingTool
    {
        public override List<string> Layers => ["Foreground", "Background"];

        internal List<string> favouriteFG = [];
        internal List<string> favouriteBG = [];

        public override Dictionary<string, string> GetMaterials()
        {
            Dictionary<string, string> materials = [];
            var list = selectedLayer == 0 ? favouriteFG : favouriteBG;
            var tiles = selectedLayer == 0 ? MainPlugin.Instance.fgTiles : MainPlugin.Instance.bgTiles;
            foreach (string material in list)
            {
                if (MappingTab.searchTerm != "" && !tiles[material].name.ToLower().Contains(MappingTab.searchTerm))
                    continue;
                materials[material] = "★ " + tiles[material].name;
            }

            foreach (var pair in tiles)
            {
                if ((selectedLayer == 0 ? favouriteFG : favouriteBG).Contains(pair.Key))
                    continue;
                if (MappingTab.searchTerm != "" && !pair.Value.name.ToLower().Contains(MappingTab.searchTerm))
                    continue;
                materials[pair.Key] = pair.Value.name;
            }
            return materials;
        }

        public override void OnFavourited(string material)
        {
            var list = selectedLayer == 0 ? favouriteFG : favouriteBG;
            if (list.Contains(material))
            {
                list.Remove(material);
            }
            else
            {
                list.Add(material);
            }
        }

        public override void LoadFavourites(JObject data)
        {
            JArray bgTiles = data.Value<JArray>("bgTiles");
            JArray fgTiles = data.Value<JArray>("fgTiles");
            if (bgTiles == null || fgTiles == null)
                return;

            foreach (string key in bgTiles)
            {
                favouriteBG.Add(key);
            }
            foreach (string key in fgTiles)
            {
                favouriteFG.Add(key);
            }
        }

        public override JToken SaveFavourites()
        {
            JObject favourites = new JObject()
            {
                {"bgTiles", JToken.FromObject(favouriteBG)},
                {"fgTiles", JToken.FromObject(favouriteFG)}
            };
            return favourites;
        }

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