using System;
using System.Collections;
using System.Collections.Generic;

namespace Edelweiss.Interop
{
    public class BindableList<T>() : BindableVariable<List<T>>([]), IEnumerable<T>
    {
        public event Action<T> ItemAdded;
        public event Action<T> ItemRemoved;
        public event Action<int, T> ItemChanged;
        
        public void Add(T item)
        {
            Value.Add(item);
            ItemAdded?.Invoke(item);
        }

        public void Remove(T item)
        {
            Value.Remove(item);
            ItemRemoved?.Invoke(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T this[int i]
        {
            get => Value[i];
            set
            {
                Value[i] = value;
                ItemChanged?.Invoke(i, value);
            }
        }
    }
}