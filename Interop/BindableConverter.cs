using System;
using System.Reflection;
using Edelweiss.Utils;
using Newtonsoft.Json;

namespace Edelweiss.Interop
{
    public class BindableConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition().IsSubclassOfRawGeneric(typeof(BindableVariable<>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object obj = Activator.CreateInstance(objectType);
            objectType.GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, reader.Value);
            return obj;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.GetType().GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(value));
        }
    }
}