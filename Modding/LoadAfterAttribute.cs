using System;

namespace Edelweiss.Plugins
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LoadAfterAttribute(params Type[] otherTypes) : Attribute
    {
        internal Type[] otherTypes = otherTypes;
    }
}