using System;

namespace Edelweiss.MVC.Controllers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ControllerAttibute(Type controllerType) : Attribute
    {
        public Type ControllerType = controllerType;
    }
}