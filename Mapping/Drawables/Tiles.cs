using System.Drawing;
using System.Linq;
using Edelweiss.Utils;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Drawables
{
    /// <summary>
    /// A class that draws tile data
    /// </summary>
    public class Tiles : Drawable
    {
        /// <summary>
        /// Whether this object draws foreground tiles
        /// </summary>
        public bool foreground;

        /// <summary>
        /// The dimensions in tiles of the drawable
        /// </summary>
        public int width, height;

        /// <summary>
        /// The position of the drawable in pixels
        /// </summary>
        public int x, y;

        /// <summary>
        /// The tile data this drawable contains
        /// </summary>
        public string data;

        /// <summary>
        /// The opacity of the tile object. 1 is fully opaque and 0 is fully transparent
        /// </summary>
        public float opacity;

        /// <summary>
        /// 
        /// </summary>
        public Tiles()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public Tiles(Table table)
        {
            x = (int)table.Get<double>("x");
            y = (int)table.Get<double>("y");
            width = (int)table.Get<double>("width");
            height = (int)table.Get<double>("height");
            data = table.Get<string>("data");
            foreground = table.Get<bool>("foreground");
            opacity = (float)table.Get<double>("opacity", 1);
            depth = (int)table.Get<double>("depth");
        }

        /// <summary>
        /// Creates a drawable tiles object with the given parameters
        /// </summary>
        /// <param name="tile">The tile data to use</param>
        /// <param name="x">The x-coordinate of the drawable in pixels</param>
        /// <param name="y">The y-coordinate of the drawable in pixels</param>
        /// <param name="tileWidth">The width of the drawable in tiles</param>
        /// <param name="tileHeight">The height of the drawable in tiles</param>
        /// <param name="opacity">The opacity of the drawable</param>
        public Tiles(TileData tile, int x, int y, int tileWidth, int tileHeight, float opacity = 1f) : this(tile.ID, !tile.path.StartsWith("bg"), x, y, tileWidth, tileHeight, opacity)
        {
        }

        /// <summary>
        /// Creates a drawable tiles object with the given parameters
        /// </summary>
        /// <param name="tileID">The ID of the tile to use</param>
        /// <param name="foreground">Whether the tile is a foreground tile</param>
        /// <param name="x">The x-coordinate of the drawable in pixels</param>
        /// <param name="y">The y-coordinate of the drawable in pixels</param>
        /// <param name="tileWidth">The width of the drawable in tiles</param>
        /// <param name="tileHeight">The height of the drawable in tiles</param>
        /// <param name="opacity">The opacity of the drawable</param>
        public Tiles(string tileID, bool foreground, int x, int y, int tileWidth, int tileHeight, float opacity = 1f)
        {
            this.x = x;
            this.y = y;
            width = tileWidth;
            height = tileHeight;
            data = string.Concat(Enumerable.Repeat(tileID, width * height));
            this.foreground = foreground;
            this.opacity = opacity;
        }

        /// <inheritdoc/>
        public override void Draw()
        {
            if (SpriteDestination.destination == null)
                return;

            SpriteDestination.destination.Add(new JObject()
            {
                { "type", "tiles" },
                { "tiles", foreground ? "Edelweiss:ForegroundTiles" : "Edelweiss:BackgroundTiles" },
                { "x", x - SpriteDestination.offsetX },
                { "y", y - SpriteDestination.offsetY },
                { "width", width },
                { "height", height },
                { "depth", depth },
                { "tileData", data },
                { "connectToOutOfBounds", false },
                { "opacity", opacity }
            });
        }

        /// <inheritdoc/>
        public override Rectangle Bounds()
        {
            return new Rectangle(x, y, width, height);
        }

        /// <inheritdoc/>
        public override Table ToLuaTable(Script script)
        {
            Table table = base.ToLuaTable(script);

            table["data"] = data;
            table["foreground"] = foreground;
            table["width"] = width;
            table["height"] = height;
            table["x"] = x;
            table["y"] = y;
            table["opacity"] = opacity;
            table["depth"] = depth;
            table["_type"] = Name;

            return table;
        }
    }
}