using System;
using System.Collections.Generic;
using System.Reflection;
using Edelweiss.MVC.Controllers;
using Edelweiss.Utils;

namespace Edelweiss.MVC
{
    public class Model : Syncable
    {
        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> propertyLookup = [];
        private static readonly Dictionary<Type, Dictionary<string, EventInfo>> eventLookup = [];
        private readonly object Value;

        public static event Action<Type, Model> ModelCreated;
        public readonly Controller Controller;

        private Model(object value)
        {
            Value = value;
            if(!propertyLookup.ContainsKey(value.GetType()))
            {
                // Generate property lookup
                Dictionary<string, PropertyInfo> properties = [];
                propertyLookup[value.GetType()] = properties;
                foreach(PropertyInfo property in value.GetType().GetProperties())
                {
                    if(property.TryGetCustomAttribute(true, out ModelPropertyAttribute attr))
                    {
                        properties[attr.Name ?? property.Name] = property;
                    }
                }

                // Generate event lookup
                Dictionary<string, EventInfo> events = [];
                eventLookup[value.GetType()] = events;
                foreach(EventInfo evt in value.GetType().GetEvents())
                {
                    if(evt.TryGetCustomAttribute(true, out ModelPropertyAttribute attr))
                    {
                        events[attr.Name ?? evt.Name] = evt;
                    }
                }
            }

            if(value.GetType().TryGetCustomAttribute(true, out CustomControllerAttribute controllerAttibute) && controllerAttibute.ControllerType.IsAssignableTo(typeof(Controller)))
            {
                Controller = Controller.Create(controllerAttibute.ControllerType, this);
            }
            else
            {
                Controller = Controller.Create(typeof(DefaultController), this);
            }
        }

        public object Get(string name) => propertyLookup[Value.GetType()].GetValueOrDefault(name)?.GetValue(Value);
        public void Set(string name, object value) => propertyLookup[Value.GetType()].GetValueOrDefault(name)?.SetValue(Value, value);
        public void Subscribe(string eventName, Delegate callback)
        {
            EventInfo evt = eventLookup[Value.GetType()].GetValueOrDefault(eventName);
            Type handlerType = evt.EventHandlerType;
            if(handlerType.IsAssignableFrom(callback.GetType()))
            {
                evt.AddEventHandler(Value, callback);
            }
            else
            {
                throw new ArgumentException($"Invalid delegate type for event {eventName}");
            }
        }

        public static Model Create(object value)
        {
            Model model = new Model(value);
            ModelCreated?.Invoke(value.GetType(), model);
            return model;
        }
    }
}