using System;
using System.Linq;

namespace Edelweiss.Interop
{
    /// <summary>
    /// Extension methods relating to interops
    /// </summary>
    public static class InteropExtensions
    {
        /// <summary>
        /// Event invoked when a syncable is synced with the frontend
        /// </summary>
        public static event Action<string, object> OnSync;

        /// <summary>
        /// Sync a syncable with the frontend
        /// </summary>
        public static void Sync(this ISyncable self)
        {
            ISyncable.Syncables[self.Name()] = self;
            OnSync?.Invoke(self.Name(), self);
        }

        /// <summary>
        /// Makes one bindable list be a Select operation on another list while automatically syncing them
        /// </summary>
        /// <param name="output"></param>
        /// <param name="source">The source list</param>
        /// <param name="transform">The transformation to apply to each element of the source list</param>
        public static void MakeTransform<T, U>(this BindableList<U> output, BindableList<T> source, Func<T, U> transform)
        {
            source.ItemAdded += value => output.Add(transform(value));
            source.ItemRemoved += (i, value) => output.Remove(transform(value));
            source.ItemChanged += (i, value) => output[i] = transform(value);
            source.ValueChanged += (value) => output.Value = source.Select(transform).ToList();
            source.Value = source.Value;
        }

        /// <summary>
        /// Makes one bindable list be a Select operation on a dictionary while automatically syncing them
        /// </summary>
        /// <param name="output"></param>
        /// <param name="source">The source dictionary</param>
        /// <param name="transform">The transformation to apply to each key and value of the source dictionary</param>
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