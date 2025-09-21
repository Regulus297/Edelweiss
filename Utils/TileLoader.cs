using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;
using Edelweiss.Plugins;

namespace Edelweiss.Utils
{
    /// <summary>
    /// The class responsible for loading tiles
    /// </summary>
    public static class TileLoader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, TileData> LoadTileXML(string file)
        {
            Dictionary<string, TileData> tiles = [];
            tiles[" "] = new TileData(" ", "air", [], "");

            XmlDocument xmlDocument = new();
            xmlDocument.Load(file);

            XmlElement root = xmlDocument.DocumentElement;
            foreach (XmlNode tileset in root.SelectNodes("Tileset"))
            {
                string id = tileset.Attributes["id"].Value;
                string path = tileset.Attributes["path"].Value;
                string copy = tileset.Attributes["copy"]?.Value ?? "";
                string ignores = tileset.Attributes["ignores"]?.Value ?? "";
                Dictionary<string, List<Point>> maskToTiles = [];
                if (copy == "")
                {
                    foreach (XmlNode pattern in tileset.SelectNodes("set"))
                    {
                        string mask = pattern.Attributes["mask"].Value;
                        string tileString = pattern.Attributes["tiles"].Value;
                        List<Point> tileCoords = [];
                        foreach (string coord in tileString.Split(";"))
                        {
                            int tileX = int.Parse(coord.Split(",")[0]);
                            int tileY = int.Parse(coord.Split(",")[1]);
                            tileCoords.Add(new Point(tileX, tileY));
                        }
                        maskToTiles[mask] = tileCoords;
                    }
                }
                else
                {
                    maskToTiles = tiles[copy].masks;
                }
                tiles[id] = new TileData(id, path, maskToTiles, ignores);
            }

            return tiles;
        }
    }

    /// <summary>
    /// Contains data for a tile
    /// </summary>
    public class TileData
    {
        /// <summary>
        /// The ID of the tile
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// The path to the texture of the tile
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// The x-position of the tileset in the atlas
        /// </summary>
        public int atlasX { get; set; }

        /// <summary>
        /// The y-position of the tileset in the atlas
        /// </summary>
        public int atlasY { get; set; }

        /// <summary>
        /// The patterns of the tile
        /// </summary>
        public Dictionary<string, List<Point>> masks { get; set; }
        /// <summary>
        /// Which tiles, if any, that the tile ignores
        /// </summary>
        public string ignores { get; set; }

        /// <summary>
        /// The display name of the tile
        /// </summary>
        public string name { get; set; }

        /// <param name="ID">The ID of the tile</param>
        /// <param name="path">The path to the texture of the tile relative to Gameplay/tilesets/</param>
        /// <param name="masks">The patterns of the tile</param>
        /// <param name="ignores">Which tiles, if any, that the tile ignores</param>
        public TileData(string ID, string path, Dictionary<string, List<Point>> masks, string ignores = "")
        {
            this.ID = ID;
            this.path = "Gameplay/tilesets/" + path;
            this.masks = masks;
            this.ignores = ignores;
            name = (path.StartsWith("bg") ? path.Substring(2) : path).CamelCaseToText();

            TextureData data = CelesteModLoader.GetTextureData(this.path);
            if (data == null)
                return;

            atlasX = data.atlasX;
            atlasY = data.atlasY;
        }
    }
}