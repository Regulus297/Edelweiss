using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Edelweiss.Mapping.Drawables;
using Edelweiss.Mapping.Tools;
using Edelweiss.Plugins;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities.Helpers
{
    /// <summary>
    /// Class that assists in making fake tile entities
    /// </summary>
    public sealed class TileHelper : LoadedType
    {
        internal static Dictionary<string, string> fgids = [], bgids = [];
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
            return [new Tiles(entity.Get(key, "3"), foreground, entity.x, entity.y, entity.width / 8, entity.height / 8, opacity) {
                depth = entity.depth
            }];
        }

        /// <summary>
        /// Gets the merged sprite for multiple fake tile entities
        /// </summary>
        public static List<Drawable> GetMergedSprite(IEnumerable<Entity> entities, string key, bool foreground = true, float opacity = 1)
        {
            int minX = int.MaxValue, minY = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue;
            foreach (Entity entity in entities)
            {
                minX = Math.Min(minX, entity.x / 8);
                minY = Math.Min(minY, entity.y / 8);
                maxX = Math.Max(maxX, (entity.x + entity.width) / 8);
                maxY = Math.Max(maxY, (entity.y + entity.height) / 8);
            }
            int w = maxX - minX;
            int h = maxY - minY;

            string[] ids = Enumerable.Repeat(" ", w * h).ToArray();
            foreach (Entity entity in entities)
            {
                for (int x = 0; x < entity.width / 8; x++)
                {
                    for (int y = 0; y < entity.height / 8; y++)
                    {
                        int cx = -minX + x + entity.x / 8;
                        int cy = -minY + y + entity.y / 8;
                        ids[cx + w * cy] = entity.Get(key, "3");
                    }
                }
            }
            Tiles tiles = new Tiles()
            {
                data = string.Concat(ids),
                x = minX * 8,
                y = minY * 8,
                width = w,
                height = h,
                foreground = true,
                opacity = opacity
            };

            return [tiles];
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool Cycle(Entity entity, string key, int amount, bool foreground = true)
        {
            entity[key] = GetCycleValue(entity.Get<string>(key).ToString(), amount, foreground);
            return true;
        }

        /// <summary>
        /// Gets the field information for the entity
        /// </summary>
        /// <param name="key">The key currently being asked for</param>
        /// <param name="tileKey">The key representing the tile for the entity</param>
        /// <param name="foreground"></param>
        /// <param name="allowAir"></param>
        /// <returns></returns>
        public static JObject GetFieldInformation(string key, string tileKey, bool foreground = true, bool allowAir = false)
        {
            if (key != tileKey)
                return null;

            JObject tiles = [];

            foreach (var pair in foreground ? fgids : bgids)
            {
                if (allowAir || pair.Key != " ")
                {
                    tiles.Add(pair.Value, pair.Key);
                }
            }
                
            return new JObject()
            {
                {"items", tiles}
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public static string GetCycleValue(string current, int amount, bool foreground = true)
        {
            current = (foreground ? fgids : bgids).Keys.ToList().Cycle(current.ToString(), amount);
            if (current.ToString() == " ")
            {
                current = (foreground ? fgids : bgids).Keys.ToList().Cycle(current.ToString(), amount);
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
            foreach (var key in foreground ? MainPlugin.Instance.fgTiles : MainPlugin.Instance.bgTiles)
            {
                (foreground ? fgids : bgids).Add(key.Key, key.Value.name);
            }
        }
    }
}