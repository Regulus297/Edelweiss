using System;
using System.Collections.Generic;
using System.Reflection;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;
using MoonSharp.Interpreter;

namespace Edelweiss.Mapping.Drawables
{
    [BaseRegistryObject()]
    public abstract class Drawable : PluginRegistryObject
    {
        private static Dictionary<string, Type> drawables = [];
        public abstract void Draw();

        public sealed override void Load()
        {
            drawables[Name] = GetType();
        }

        public static Drawable FromTable(Table table)
        {
            string type = table.Get("_type").String;
            if (!drawables.TryGetValue(type, out Type drawableType))
                throw new KeyNotFoundException($"Drawable type '{type}' does not exist.");

            ConstructorInfo constructor = drawableType.GetConstructor([typeof(Table)]);
            if (constructor == null)
                throw new MissingMethodException($"Drawable type '{drawableType}' does not define a constructor that accepts a table.");

            return (Drawable)constructor.Invoke([table]);
        }
    }
}