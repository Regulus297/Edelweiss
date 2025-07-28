using System;
using System.Collections.Generic;

namespace Edelweiss.RegistryTypes
{
    /// <summary>
    /// A list of registry objects.
    /// Stores an integer ID, an instance of the type, and the type itself.
    /// </summary>
    /// <typeparam name="T">The type that is stored in the list</typeparam>
    public class RegistryList<T>
    {
        internal List<T> values = [];
        Dictionary<T, int> inverseLookup = [];
        Dictionary<Type, T> instanceList = [];

        /// <summary>
        /// Adds an instance to the list
        /// </summary>
        /// <param name="value">An instance of the object to add</param>
        public void Add(T value)
        {
            values.Add(value);
            inverseLookup[value] = values.Count - 1;
            instanceList[value.GetType()] = value;
        }
        
        /// <summary>
        /// Gets the instance of a particular ID
        /// </summary>
        public T GetValue(int i) => values[i];

        /// <summary>
        /// Gets the instance of a particular type
        /// </summary>
        public T GetValue(Type type) => instanceList[type];

        /// <summary>
        /// Gets the instance of a particular type and casts it to that type
        /// </summary>
        public U GetValue<U>() where U : class, IRegistryObject => GetValue(typeof(U)) as U;

        /// <summary>
        /// Returns true if the given type is contained in the registry list, else false.
        /// </summary>
        public bool ContainsType(Type type) => instanceList.ContainsKey(type);

        /// <summary>
        /// Gets the ID of a particular instance
        /// </summary>
        public int GetIndex(T value) => inverseLookup[value];

        /// <summary>
        /// The number of entries in the list
        /// </summary>
        public int Count => values.Count;
    }
}