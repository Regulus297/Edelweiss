using System;
using System.Collections;
using System.Collections.Generic;

namespace Edelweiss.Interop
{
    /// <summary>
    /// A variable that contains a list, invoking events when items are added, removed or changed
    /// </summary>
    /// <typeparam name="T">The type of elements in the list</typeparam>
    public class BindableList<T>() : BindableVariable<List<T>>([]), IEnumerable<T>
    {
        /// <summary>
        /// Invoked when an item is added
        /// </summary>
        public event Action<T> ItemAdded;

        /// <summary>
        /// Invoked when an item is removed: passes in the index and item
        /// </summary>
        public event Action<int, T> ItemRemoved;

        /// <summary>
        /// Invoked when an item at an index is changed: passes in the index and item
        /// </summary>
        public event Action<int, T> ItemChanged;
        
        /// <summary>
        /// Adds an item to the list, invoking ItemAdded
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(T item)
        {
            if(item == null)
            {
                Add();
                return;
            }
            Value.Add(item);
            if(!suppressed)
                ItemAdded?.Invoke(item);
        }

        /// <summary>
        /// Adds an item to the list by creating an instance
        /// </summary>
        public void Add()
        {
            Add(Activator.CreateInstance<T>());
        }

        /// <summary>
        /// Removes the given item from the list, invoking ItemRemoved
        /// </summary>
        /// <param name="item"></param>
        public void Remove(T item)
        {
            int index = Value.IndexOf(item);
            if(Value.Remove(item) && !suppressed)
                ItemRemoved?.Invoke(index, item);
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Setting will invoke ItemChanged
        /// </summary>
        public T this[int i]
        {
            get => Value[i];
            set
            {
                Value[i] = value;
                if(!suppressed)
                    ItemChanged?.Invoke(i, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static implicit operator BindableList<T>(List<T> value) => new()
        {
            Value = value
        };
    }
}