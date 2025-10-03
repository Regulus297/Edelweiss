using System.Collections;
using System.Collections.Generic;

namespace Edelweiss.Mapping.SaveLoad
{
    /// <summary>
    /// A lookup table for strings
    /// </summary>
    public class StringLookup : IEnumerable<KeyValuePair<string, short>>
    {
        Dictionary<string, short> values = [];
        short count = 0;

        /// <summary>
        /// The number of entries in the lookup
        /// </summary>
        public int Count => values.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public short this[string v]
        {
            get
            {
                return values.GetValueOrDefault(v, (short)0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, short>> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        /// <summary>
        /// Adds an element to the lookup
        /// </summary>
        /// <param name="value"></param>
        public void Add(string value)
        {
            if (!values.ContainsKey(value))
                values[value] = count++;
        }

        /// <summary>
        /// Adds several elements to the lookup
        /// </summary>
        /// <param name="values"></param>
        public void Add(params string[] values)
        {
            foreach (string v in values)
            {
                Add(v);
            }
        }

        /// <summary>
        /// Determines whether the lookup contains the specified key
        /// </summary>
        public bool ContainsKey(string value) => values.ContainsKey(value);

        /// <summary>
        /// Clears the lookup
        /// </summary>
        public void Clear()
        {
            values.Clear();
            count = 0;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}