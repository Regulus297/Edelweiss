using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Edelweiss.RegistryTypes;

namespace Edelweiss.Plugins
{
    public static class PluginLoader
    {
        public static void LoadPlugins()
        {
            LoadAssembly(Assembly.GetExecutingAssembly());
            string directory = Path.Join(Directory.GetCurrentDirectory(), "Plugins");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            foreach (string modDirectory in Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly))
            {
                if (ValidateModDirectory(modDirectory, out string modFilePath))
                {
                    Assembly assembly = Assembly.LoadFile(modFilePath);
                    Plugin plugin = LoadAssembly(assembly);
                }
            }

            Registry.ForAll<Plugin>(plugin => plugin.PostSetupContent());
        }

        private static bool ValidateModDirectory(string modDirectory, out string modFilePath)
        {
            modFilePath = "";
            string[] files = Directory.GetFiles(modDirectory, "*.dll");
            if (files.Length > 0)
            {
                modFilePath = files[0];
            }
            else
            {
                return false;
            }
            return true;
        }

        public static Plugin LoadAssembly(Assembly assembly)
        {
            LoadBaseRegistryObjects(assembly);
            Plugin plugin = null;
            foreach (Type type in assembly.GetTypes())
            {
                if (!type.IsAbstract && type.IsAssignableTo(typeof(Plugin)))
                {
                    plugin = (Plugin)Activator.CreateInstance(type);
                    Console.WriteLine(plugin.ID);
                    Registry.registry[typeof(Plugin)].Add(plugin);
                    plugin.OnRegister();
                }
            }
            foreach (Type type in assembly.GetTypes())
            {
                foreach (Type baseType in Registry.registry.Keys)
                {
                    if (!type.IsAbstract && type.GetInterfaces().Any(i => i == typeof(IRegistryObject)) && type.IsAssignableTo(baseType) && !type.IsAssignableTo(typeof(Plugin)))
                    {
                        IRegistryObject instance = (IRegistryObject)Activator.CreateInstance(type);
                        if (instance is PluginRegistryObject o)
                        {
                            o.Plugin = plugin;
                        }

                        Registry.registry[baseType].Add(instance);
                        instance.OnRegister();
                        break;
                    }
                }
            }
            return plugin;
        }

        public static void LoadBaseRegistryObjects(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsAbstract && type.CustomAttributes.Any(a => a.AttributeType == typeof(BaseRegistryObject)))
                {
                    Registry.registry[type] = new();
                }
            }
        }
    }
}