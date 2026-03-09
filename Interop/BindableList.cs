using System;
using System.Collections;
using System.Collections.Generic;

namespace Edelweiss.Interop
{
    public class BindableList<T>() : BindableVariable<List<T>>([]), IEnumerable<T>
    {
        public event Action<T> ItemAdded;
        public event Action<int, T> ItemRemoved;
        public event Action<int, T> ItemChanged;
        
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

        public void Add()
        {
            Add(Activator.CreateInstance<T>());
        }

        public void Remove(T item)
        {
            int index = Value.IndexOf(item);
            Value.Remove(item);
            if(!suppressed)
                ItemRemoved?.Invoke(index, item);
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
                if(!suppressed)
                    ItemChanged?.Invoke(i, value);
            }
        }
    }
}