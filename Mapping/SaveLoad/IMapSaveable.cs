using System.IO;

namespace Edelweiss.Mapping.SaveLoad
{
    /// <summary>
    /// Interface implemented by objects that can be added to a string lookup
    /// </summary>
    public interface IMapSaveable
    {
        /// <summary>
        /// Adds the attributes of the object to the lookup
        /// </summary>
        public void AddToLookup(StringLookup lookup);

        /// <summary>
        /// Saves the object to the writer
        /// </summary>
        public void Encode(BinaryWriter writer);

        /// <summary>
        /// Decodes the object from the given element
        /// </summary>
        /// <param name="element"></param>
        public void Decode(MapElement element);
    }
}