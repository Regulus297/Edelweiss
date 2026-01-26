using System;

namespace Edelweiss.MVC
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Event)]
    public class ModelPropertyAttribute(string Name = null) : Attribute
    {
        public string Name = Name;
    }
}