using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Edelweiss.Loenn;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;
using MoonSharp.Interpreter;

namespace Edelweiss.Mapping.Drawables
{
    /// <summary>
    /// The base type for any object that can be drawn to the frontend.
    /// All inheriting objects must define a parameterless constructor and a constructor accepting a table.
    /// </summary>
    [BaseRegistryObject()]
    public abstract class Drawable : PluginRegistryObject, ILuaConvertible
    {
        private static Dictionary<string, Type> drawables = [];

        /// <summary>
        /// The render depth of the drawable. Positive values are further back and negative values are further forward.
        /// </summary>
        public int depth;

        /// <summary>
        /// Draws the object to the current SpriteDestination.
        /// </summary>
        public abstract void Draw();

        /// <summary>
        /// Gets the rectangle that covers the drawable
        /// </summary>
        public virtual Rectangle Bounds()
        {
            return Rectangle.Empty;
        }


        /// <inheritdoc/>
        public sealed override void Load()
        {
            drawables[Name] = GetType();
        }

        /// <summary>
        /// Creates a Drawable from a given Lua table depending on the _type defined in the table.
        /// </summary>
        /// <exception cref="KeyNotFoundException">Thrown if the _type in the table is not a defined Drawable.</exception>
        /// <exception cref="MissingMethodException">Thrown if the found Drawable type does not have a constructor accepting a table.</exception>
        public static Drawable FromTable(Table table)
        {
            string type = table?.Get("_type")?.String;
            if (type == null)
            {
                return new EmptyDrawable();
            }
            if (!drawables.TryGetValue(type, out Type drawableType))
                throw new KeyNotFoundException($"Drawable type '{type}' does not exist.");

            ConstructorInfo constructor = drawableType.GetConstructor([typeof(Table)]);
            if (constructor == null)
                throw new MissingMethodException($"Drawable type '{drawableType}' does not define a constructor that accepts a table.");

            return (Drawable)constructor.Invoke([table]);
        }

        /// <summary>
        /// Converts the drawable to a table
        /// </summary>
        /// <param name="script">The script the table should belong to</param>
        public virtual Table ToLuaTable(Script script)
        {
            Table table = new(script);
            table["depth"] = depth;
            table["draw"] = () =>
            {
                FromTable(table).Draw();
            };
            return table;
        }
    }
}