using System;
using System.Collections.Generic;
using System.Reflection;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace Edelweiss.Plugins
{
    /// <summary>
    /// Handles all mod hooks
    /// </summary>
    public static class HookManager
    {

        internal static List<Hook> onHooks = [];
        internal static List<ILHook> ILHooks = [];
        internal static Dictionary<string, MethodInfo> methods = [];
        internal static void LoadMethods()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
                {
                    if (method.GetBaseDefinition().DeclaringType == typeof(object) || method.IsGenericMethod)
                        continue;
                    string identifier = type.Name + "." + method.Name;
                    methods[identifier] = method;
                }
            }
        }

        /// <summary>
        /// Registers an on hook
        /// </summary>
        /// <typeparam name="T">The type of the delegate (usually autodetected by the compiler and IDE)</typeparam>
        /// <param name="method">The key of the method: takes the form "TypeName.MethodName"</param>
        /// <param name="hook">The hook itself</param>
        public static void AddHook<T>(string method, T hook) where T : Delegate
        {
            onHooks.Add(new Hook(methods[method], hook));
        }

        /// <summary>
        /// Registers an IL hook
        /// </summary>
        /// <param name="method">The method itself (useful for hooking into other mods)</param>
        /// <param name="hook">The IL manipulator</param>
        public static void AddILHook(string method, ILContext.Manipulator hook)
        {
            ILHooks.Add(new ILHook(methods[method], hook));
        }

        /// <summary>
        /// Registers an on hook
        /// </summary>
        /// <typeparam name="T">The type of the delegate (usually autodetected by the compiler and IDE)</typeparam>
        /// <param name="method">A reference to the method itself (only works for static methods)</param>
        /// <param name="hook">The hook itself</param>
        public static void AddHook<T>(Delegate method, T hook) where T : Delegate
        {
            onHooks.Add(new Hook(method.Method, hook));
        }

        /// <summary>
        /// Registers an on hook
        /// </summary>
        /// <typeparam name="T">The type of the delegate (usually autodetected by the compiler and IDE)</typeparam>
        /// <param name="method">The method itself (useful for hooking into other mods)</param>
        /// <param name="hook">The hook itself</param>
        public static void AddHook<T>(MethodBase method, T hook) where T : Delegate
        {
            onHooks.Add(new Hook(method, hook));
        }
    }
}