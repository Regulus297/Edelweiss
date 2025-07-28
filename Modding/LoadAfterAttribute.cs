using System;

namespace Edelweiss.Plugins
{
    /// <summary>
    /// Classes marked with this attribute will require that the specified types be loaded before they can be loaded
    /// </summary>
    /// <param name="otherTypes">The types to load before this type</param>
    [AttributeUsage(AttributeTargets.Class)]
    public class LoadAfterAttribute(params Type[] otherTypes) : Attribute
    {
        internal Type[] otherTypes = otherTypes;
    }
}