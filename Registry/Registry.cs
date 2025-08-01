using System;
using System.Collections.Generic;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;

namespace Edelweiss.RegistryTypes
{
    /// <summary>
    /// Contains all registry objects categorised into their respective registry lists
    /// </summary>
    public static class Registry
    {
        /// <summary>
        /// The whole registry
        /// </summary>
        public readonly static Dictionary<Type, RegistryList<IRegistryObject>> registry = [];
        public readonly static List<Type> allRegisteredTypes = [];

        /// <summary>
        /// Performs an action for all registered objects of a given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        public static void ForAll<T>(Action<T> action) where T : IRegistryObject
        {
            foreach (IRegistryObject obj in registry[typeof(T)].values)
            {
                action((T)obj);
            }
        }
    }
}