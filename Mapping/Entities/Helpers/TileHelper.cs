using System.Collections.Generic;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Tools;
using Edelweiss.Plugins;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Entities
{
    /// <summary>
    /// Class that assists in making fake tile entities
    /// </summary>
    public sealed class TileHelper : LoadedType
    {
        internal static List<string> fgids = [], bgids = [];
        /// <summary>
        /// Gets the currently selected tile
        /// </summary>
        /// <param name="fallback">The tile used if the current material is unsuitable</param>
        /// <param name="foreground">Whether to return the foreground material</param>
        /// <param name="allowAir">Whether air is a valid tile to return</param>
        /// <returns></returns>
        public static string GetMaterial(string fallback = "3", bool foreground = true, bool allowAir = false)
        {
            string mat = foreground ? TileTool.selectedFG : TileTool.selectedBG;
            if (string.IsNullOrEmpty(mat))
            {
                return fallback;
            }
            return (!allowAir && (mat == "0" || mat == " ")) ? fallback : mat;
        }

        /// <summary>
        /// Gets the sprite for a fake tile entity
        /// </summary>
        /// <param name="entity">The entity instance</param>
        /// <param name="key">The key of the placement data containing the tile ID</param>
        /// <param name="foreground">Whether this is a foreground tile</param>
        /// <param name="opacity">The opacity of the tile object</param>
        public static List<Drawable> GetSprite(Entity entity, string key, bool foreground = true, float opacity = 1)
        {
            return [new Tiles(entity[key].ToString(), foreground, entity.x, entity.y, entity.width / 8, entity.height / 8, opacity) {
                depth = entity.depth
            }];
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool Cycle(Entity entity, string key, int amount, bool foreground = true)
        {
            entity[key] = GetCycleValue(entity[key].ToString(), amount, foreground);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public static string GetCycleValue(string current, int amount, bool foreground = true)
        {
            current = (foreground ? fgids : bgids).Cycle(current.ToString(), amount);
            if (current.ToString() == " ")
            {
                current = (foreground ? fgids : bgids).Cycle(current.ToString(), amount);
            }
            return current;
        }

        /// <inheritdoc/>
        public override void Load()
        {
            MainPlugin.Instance.fgTileData.OnChanged += _ => Recache(true);
            MainPlugin.Instance.bgTileData.OnChanged += _ => Recache(false);
        }

        private static void Recache(bool foreground) {
            foreach (string key in (foreground ? MainPlugin.Instance.fgTiles : MainPlugin.Instance.bgTiles).Keys)
            {
                (foreground ? fgids : bgids).Add(key);
            }
        }
    }
}