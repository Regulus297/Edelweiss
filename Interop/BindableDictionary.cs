using System;
using System.Collections;
using System.Collections.Generic;

namespace Edelweiss.Interop
{
    /// <summary>
    /// A variable that contains a list, invoking events when items are added, removed or changed
    /// </summary>
    /// <typeparam name="T">The key type</typeparam>
    /// <typeparam name="U">The value type</typeparam>
    public class BindableDictionary<T, U>() : BindableVariable<Dictionary<T, U>>([]), IEnumerable<KeyValuePair<T, U>>
    {
        /// <summary>
        /// Invoked when an item at a key is changed
        /// </summary>
        public event Action<T, U> ItemChanged;

        /// <summary>
        /// Invoked when an item at a key is removed
        /// </summary>
        public event Action<T, U> ItemRemoved;

        /// <summary>
        /// Invoked when an item at a key is added
        /// </summary>
        public event Action<T, U> ItemAdded;

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<T, U>> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Setting will invoke ItemChanged if the key is already in the list and ItemAdded if not
        /// </summary>
        public U this[T i]
        {
            get => Value[i];
            set
            {
                if(!suppressed && Value.ContainsKey(i))
                    ItemChanged?.Invoke(i, value);
                else if(!suppressed)
                    ItemAdded?.Invoke(i, value);
                Value[i] = value;
            }
        }

        /// <summary>
        /// Adds a key and value to the dictionary
        /// </summary>
        public void Add(T key, U value)
        {
            Value.Add(key, value);
            if(!suppressed)
                ItemAdded?.Invoke(key, value);
        }

        /// <summary>
        /// Removes the item at a given key
        /// </summary>
        public void Remove(T key)
        {
            if(!suppressed)
                ItemRemoved?.Invoke(key, Value[key]);
            Value.Remove(key);
        }
    }
}