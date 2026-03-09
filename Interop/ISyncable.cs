using System;
using System.Collections.Generic;

namespace Edelweiss.Interop
{
    /// <summary>
    /// Interface all classes that need to sync with the UI must implement
    /// </summary>
    public interface ISyncable
    {
        /// <summary>
        /// Dictionary of all syncable keys to their values
        /// </summary>
        public readonly static Dictionary<string, object> Syncables = [];

        /// <summary>
        /// The key this syncable should be accessible with
        /// </summary>
        public string Name();
    }
}