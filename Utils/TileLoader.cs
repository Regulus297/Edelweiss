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

    public class TileData(string ID, string path, Dictionary<string, List<Point>> masks, string ignores = "")
    {
        public string ID = ID;
        public string path = "Gameplay/tilesets/" + path;
        public Dictionary<string, List<Point>> masks = masks;
        public string ignores = ignores;
        public string name = (path.StartsWith("bg") ? path.Substring(2) : path).CamelCaseToText();
    }
}