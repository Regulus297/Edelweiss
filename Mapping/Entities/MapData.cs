using System.Collections.Generic;
using System.IO;
using Edelweiss.Mapping.SaveLoad;

namespace Edelweiss.Mapping.Entities
{
    /// <summary>
    /// Class containing map data
    /// </summary>
    public class MapData : IMapSaveable
    {

        /// <summary>
        /// The rooms that the map contains
        /// </summary>
        public List<RoomData> rooms = [];

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, Entity> allEntities = [];

        /// <summary>
        /// The metadata for this map
        /// </summary>
        public MapMeta meta = new MapMeta();

        /// <inheritdoc/>
        public void AddToLookup(StringLookup lookup)
        {
            lookup.Add("Map", "levels", "Style", "meta", "_package");
            foreach (RoomData room in rooms)
            {
                room.AddToLookup(lookup);
            }
            meta.AddToLookup(lookup);
        }

        /// <inheritdoc/>
        public void Encode(BinaryWriter writer)
        {
            writer.WriteLookupString("Map");

            // Attribute count
            writer.Write((byte)1);
            writer.WriteAttribute("_package", "");

            // Child count
            writer.Write((short)2);

            // levels child
            writer.WriteLookupString("levels");
            writer.Write((byte)0); // Attr count
            writer.Write((short)rooms.Count); // Child count
            foreach (RoomData room in rooms)
            {
                room.Encode(writer);
            }

            meta.Encode(writer);
        }
    }
}