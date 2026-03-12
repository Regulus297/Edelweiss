using System;
using System.Collections.Generic;
using System.Reflection;
using Edelweiss.Utils;
using Newtonsoft.Json;

namespace Edelweiss.Interop
{
    /// <summary>
    /// A JSON serializer for BindableVariable that serializes just the value.
    /// </summary>
    public class BindableConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition().IsSubclassOfRawGeneric(typeof(BindableVariable<>));
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object obj = Activator.CreateInstance(objectType);
            objectType.GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, serializer.Deserialize(reader, GetBindableVariableType(objectType)));
            return obj;
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.GetType().GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(value));
        }

        private Type GetBindableVariableType(Type type)
        {
            if(type.GetGenericTypeDefinition().IsSubclassOfRawGeneric(typeof(BindableList<>)))
            {
                return typeof(List<>).MakeGenericType(type.GenericTypeArguments);
            }
            if (type.GetGenericTypeDefinition().IsSubclassOfRawGeneric(typeof(BindableDictionary<,>)))
            {
                return typeof(Dictionary<,>).MakeGenericType(type.GenericTypeArguments);
            }
            return type.GenericTypeArguments[0];
        }
    }
}