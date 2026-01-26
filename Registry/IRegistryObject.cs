using System;

namespace Edelweiss.RegistryTypes
{
    /// <summary>
    /// An interface that all types to be stored in the registry must implement
    /// </summary>
    public interface IRegistryObject
    {
        /// <summary>
        /// Called when the object is added to the registry
        /// </summary>
        public void OnRegister();
    }

    /// <summary>
    /// Types marked with this attribute are "base" objects: all types inheriting from them are stored in their list.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BaseRegistryObjectAttribute : Attribute
    {
    }
}