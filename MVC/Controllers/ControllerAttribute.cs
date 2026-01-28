using System;

namespace Edelweiss.MVC.Controllers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomControllerAttribute(Type controllerType) : Attribute
    {
        public Type ControllerType = controllerType;
    }
}