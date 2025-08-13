using System.Collections.Generic;
using System.Linq;
using Edelweiss.Preferences;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Tools
{
    /// <summary>
    /// A base class for any tool that places tiles
    /// </summary>
    public abstract class TileTool : MappingTool
    {
        /// <inheritdoc/>
        public override List<string> Layers => ["Foreground", "Background"];

        internal List<string> favouriteFG = [];
        internal List<string> favouriteBG = [];

        /// <inheritdoc/>
        public override Dictionary<string, string> GetMaterials()
        {
            Dictionary<string, string> materials = [];
            var list = selectedLayer == 0 ? favouriteFG : favouriteBG;
            var tiles = selectedLayer == 0 ? MainPlugin.Instance.fgTiles : MainPlugin.Instance.bgTiles;
            foreach (string material in list)
            {
                if (IsSearched(tiles[material].name))
                    materials[material] = "★ " + tiles[material].name;
            }

            foreach (var pair in tiles)
            {
                if (list.Contains(pair.Key))
                    continue;
                if (IsSearched(pair.Value.name))
                    materials[pair.Key] = pair.Value.name;
            }
            return materials;
        }

        /// <inheritdoc/>
        public override void OnFavourited(string material)
        {
            (selectedLayer == 0 ? favouriteFG : favouriteBG).Toggle(material);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override JToken SaveFavourites()
        {
            JObject favourites = new JObject()
            {
                {"bgTiles", JToken.FromObject(favouriteBG)},
                {"fgTiles", JToken.FromObject(favouriteFG)}
            };
            return favourites;
        }

        /// <summary>
        /// Sets a tile at the given coordinates to the current selected material.
        /// </summary>
        /// <param name="tileData">The tiles that room currently contains</param>
        /// <param name="room">The room</param>
        /// <param name="x">The x-coordinate of the tile</param>
        /// <param name="y">The y-coordinate of the tile</param>
        protected void SetTile(ref string tileData, JObject room, int x, int y)
        {
            if (!TileInBounds(room, x, y))
                return;

            int i = y * ((int)room["width"] / 8) + x;
            tileData = tileData.Substring(0, i) + selectedMaterial + tileData.Substring(i + 1);
        }

        /// <summary>
        /// Checks if a given tile is in bounds of a room
        /// </summary>
        /// <param name="room">The room</param>
        /// <param name="x">The x-coordinate of the tile</param>
        /// <param name="y">The y-coordinate of the tile</param>
        /// <returns>True if the tile is in bounds, false otherwise</returns>
        protected bool TileInBounds(JObject room, int x, int y)
        {
            return (x >= 0) && (x < ((int)room["width"] / 8)) && (y >= 0) && (y < ((int)room["height"] / 8));
        }
    }
}