using System;
using System.Collections.Generic;
using System.Reflection;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;
using Edelweiss.Utils;

namespace Edelweiss.MVC.Controllers
{
    public abstract class Controller
    {
        protected Model Model;

        public static Controller Create(Type type, Model model) {
            if(type.IsAssignableTo(typeof(Controller)))
            {
                Controller controller = (Controller)Activator.CreateInstance(type);
                controller.Model = model;
                return controller;
            }
            return null;
        }

        public void Execute(string name, params object[] args)
        {
            GetType().GetMethod(name).Invoke(this, args);
        }
    }
}