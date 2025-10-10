using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Edelweiss.Mapping.SaveLoad
{
    /// <summary>
    /// Represents an element from a map file tree
    /// </summary>
    public class MapElement : IEnumerable
    {
        /// <summary>
        /// The name of the element
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The attributes the element has
        /// </summary>
        public Dictionary<string, object> Attributes { get; private set; }

        /// <summary>
        /// The children the element has
        /// </summary>
        public List<MapElement> Children { get; private set; }

        /// <summary>
        /// Creates an element from the given reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static MapElement Create(BinaryReader reader)
        {
            MapElement element = new();
            element.Name = reader.ReadLookupString();

            byte attrCount = reader.ReadByte();
            element.Attributes = new Dictionary<string, object>(attrCount);
            for (int i = 0; i < attrCount; i++)
            {
                object value = reader.ReadAttribute(out string attrName);
                element.Attributes[attrName] = value;
            }

            short childCount = reader.ReadInt16();
            element.Children = new List<MapElement>(childCount);
            for (int i = 0; i < childCount; i++)
            {
                MapElement child = Create(reader);
                element.Children.Add(child);
            }
            return element;
        }

        /// <summary>
        /// Returns the value of the attribute with the given name, returning the default value for the type if null
        /// </summary>
        public T Attr<T>(string name)
        {
            if (Attributes.GetValueOrDefault(name) is T t)
                return t;
            return default;
        }
        
        /// <summary>
        /// Returns the value of the attribute with the given name.
        /// If the value is a floating point number, boxes it as float
        /// If the value is a whole number type, boxes it as int
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object ProcessedAttr(string name)
        {
            if (!Attributes.TryGetValue(name, out object value))
                return null;

            if (value is decimal || value is double || value is float)
                return Convert.ChangeType(value, typeof(float));
            if (value is byte || value is short || value is int || value is long)
                return Convert.ChangeType(value, typeof(int));
            return value;
        }

        /// <summary>
        /// Returns whether or not the element contains an attribute with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasAttr(string name)
        {
            return Attributes.ContainsKey(name);
        }

        /// <summary>
        /// Tries to get an attribute with the given name. If no such attribute exists or it is not of the desired type, returns false.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetAttr<T>(string name, out T value)
        {
            if (Attributes.TryGetValue(name, out object v))
            {
                try
                {
                    value = (T)Convert.ChangeType(v, typeof(T));
                    return true;
                }
                catch (Exception)
                {
                    value = default;
                    return false;
                }
            }
            value = default;
            return false;
        }

        /// <summary>
        /// Calls the callback with the attribute's value if the attribute exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public void AttrIf<T>(string name, Action<T> callback)
        {
            if (TryGetAttr(name, out T value))
            {
                callback(value);
            }
        }

        /// <summary>
        /// Sets the desired variable to the value of an attribute if it exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="var"></param>
        public void SetIfAttr<T>(string name, ref T var)
        {
            if (TryGetAttr(name, out T value))
            {
                var = value;
            }
        }

        /// <inheritdoc/>
        public IEnumerator GetEnumerator()
        {
            foreach (MapElement element in Children)
            {
                yield return element;
            }
        }
    }
}