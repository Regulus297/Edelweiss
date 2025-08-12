using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;

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
    /// <param name="ID">The ID of the tile</param>
    /// <param name="path">The path to the texture of the tile relative to Gameplay/tilesets/</param>
    /// <param name="masks">The patterns of the tile</param>
    /// <param name="ignores">Which tiles, if any, that the tile ignores</param>
    public class TileData(string ID, string path, Dictionary<string, List<Point>> masks, string ignores = "")
    {
        /// <summary>
        /// The ID of the tile
        /// </summary>
        public string ID { get; set; } = ID;
        /// <summary>
        /// The path to the texture of the tile
        /// </summary>
        public string path { get; set; } = "Gameplay/tilesets/" + path;

        /// <summary>
        /// The patterns of the tile
        /// </summary>
        public Dictionary<string, List<Point>> masks { get; set; } = masks;
        /// <summary>
        /// Which tiles, if any, that the tile ignores
        /// </summary>
        public string ignores { get; set; } = ignores;

        /// <summary>
        /// The display name of the tile
        /// </summary>
        public string name { get; set; } = (path.StartsWith("bg") ? path.Substring(2) : path).CamelCaseToText();
    }
}