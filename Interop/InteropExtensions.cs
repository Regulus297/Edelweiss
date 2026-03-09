using System;
using System.Linq;

namespace Edelweiss.Interop
{
    public static class InteropExtensions
    {
        public static event Action<string, object> OnSync;
        public static void Sync(this ISyncable self)
        {
            ISyncable.Syncables[self.Name()] = self;
            OnSync?.Invoke(self.Name(), self);
        }

        public static void MakeTransform<T, U>(this BindableList<U> output, BindableList<T> source, Func<T, U> transform)
        {
            source.ItemAdded += value => output.Add(transform(value));
            source.ItemRemoved += (i, value) => output.Remove(transform(value));
            source.ItemChanged += (i, value) => output[i] = transform(value);
            source.ValueChanged += (value) => output.Value = source.Select(transform).ToList();
            source.Value = source.Value;
        }

        public static void MakeTransform<T, U, V>(this BindableList<V> output, BindableDictionary<T, U> source, Func<T, U, V> transform)
        {
            source.ItemAdded += (key, value) => output.Add(transform(key, value));
            source.ItemRemoved += (i, value) => output.Remove(transform(i, value));
            source.ItemChanged += (i, value) => output.Value = source.ToList().Select(v => transform(v.Key, v.Value)).ToList();
            source.ValueChanged += (value) => output.Value = source.ToList().Select(v => transform(v.Key, v.Value)).ToList();
            source.Value = source.Value;
        }
    }
}