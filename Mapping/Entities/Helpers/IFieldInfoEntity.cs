using System.Collections.Generic;

namespace Edelweiss.Mapping.Entities.Helpers
{
    /// <summary>
    /// Interface for entities that use custom field info
    /// </summary>
    public interface IFieldInfoEntity
    {
        internal static Dictionary<string, EntityFieldInfo> fieldInfos = [];
        /// <summary>
        /// Initializes the field info
        /// </summary>
        public void InitializeFieldInfo(EntityFieldInfo fieldInfo);
    }
}