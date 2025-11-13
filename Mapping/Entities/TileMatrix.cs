using System.Linq;

namespace Edelweiss.Mapping.Entities
{
    /// <summary>
    /// Represents a grid of tiles
    /// </summary>
    public class TileMatrix(int width, int height)
    {
        /// <summary>
        /// The width of the matrix
        /// </summary>
        public int Width = width;

        /// <summary>
        /// The height of the matrix
        /// </summary>
        public int Height = height;

        /// <summary>
        /// The data in the matrix
        /// </summary>
        public string TileData = string.Concat(Enumerable.Repeat(' ', width * height));

        /// <summary>
        /// Gets the tile ID at the given x and y coordinate, returning a whitespace if the tile is empty
        /// </summary>
        public char GetTile(int x, int y, char defaultValue = ' ')
        {
            if (!InBounds(x, y))
                return defaultValue;

            return TileData[y * Width + x];
        }

        /// <summary>
        /// Gets the tile ID at the given x and y coordinate, returning 0 if the tile is empty
        /// </summary>
        public char GetTileConverted(int x, int y, char defaultValue = '0')
        {
            char tile = GetTile(x, y, defaultValue);
            if (tile == ' ')
                return '0';
            return tile;
        }

        /// <summary>
        /// Sets the tile ID at the given x and y coordinate to the provided tile
        /// </summary>
        public void SetTile(int x, int y, string tile)
        {
            if (!InBounds(x, y))
                return;

            TileData = TileData[..(y * Width + x)] + tile + TileData[(y * Width + x + 1)..];
        }

        /// <summary>
        /// Converts the internal tile data to a saveable string
        /// </summary>
        /// <returns></returns>
        public string Format()
        {
            return string.Join('\n', Enumerable.Range(0, TileData.Length / Width).Select(i => TileData.Substring(i * Width, Width))).Replace(' ', '0').TrimEnd('0', '\n');
        }

        /// <summary>
        /// Sets the matrix's tile data from a saved string
        /// </summary>
        /// <param name="tileData"></param>
        public void Decode(string tileData)
        {
            TileData = string.Join('\n', string.Join(string.Empty, tileData.Replace('0', ' ').Split('\n').Select(t => t.PadRight(Width))));
        }

        /// <summary>
        /// Determines whether the given x and y coordinates are in bounds
        /// </summary>
        public bool InBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
    }
}