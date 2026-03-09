using System;
using System.Collections;
using System.Collections.Generic;

namespace Edelweiss.Interop
{
    public class BindableDictionary<T, U>() : BindableVariable<Dictionary<T, U>>([]), IEnumerable<KeyValuePair<T, U>>
    {
        public event Action<T, U> ItemChanged;
        public event Action<T, U> ItemRemoved;
        public event Action<T, U> ItemAdded;
        public IEnumerator<KeyValuePair<T, U>> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

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

        public void Add(T key, U value)
        {
            Value.Add(key, value);
            if(!suppressed)
                ItemAdded?.Invoke(key, value);
        }

        public void Remove(T key)
        {
            if(!suppressed)
                ItemRemoved?.Invoke(key, Value[key]);
            Value.Remove(key);
        }
    }
}