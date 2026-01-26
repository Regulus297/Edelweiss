using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Edelweiss.Interop;
using Edelweiss.Network;
using Edelweiss.RegistryTypes;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Plugins
{
    public static class PluginLoader
    {
        public static event Action PostLoadTypes;
        public static event Action PostLoadPlugins;

        public static void LoadPlugins()
        {
            LoadPythonPlugins(Directory.GetCurrentDirectory());
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
                    LoadPythonPlugins(directory);
                }
            }

            Registry.ForAll<Plugin>(plugin => plugin.PostSetupContent());
            PostLoadPlugins?.Invoke();
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

                            foreach(CustomAttributeData attrData in type.GetCustomAttributesData())
                            {
                                if(attrData.AttributeType.IsAssignableTo(typeof(PluginLoadAttribute)))
                                {
                                    PluginLoadAttribute attr = (PluginLoadAttribute)type.GetCustomAttribute(attrData.AttributeType);
                                    attr.OnLoad(o);
                                    PostLoadTypes += () => {
                                        attr.PostLoadTypes(o);
                                    };
                                    PostLoadPlugins += () =>
                                    {
                                        attr.PostLoadPlugins(o);
                                    };
                                }
                            }
                        }

                        if(instance is ISyncable syncable)
                        {
                            syncable.Sync();
                        }

                        Registry.registry[baseType].Add(instance);
                        instance.OnRegister();
                        break;
                    }
                }
            }
            PostLoadTypes?.Invoke();
            return plugin;
        }

        public static void LoadPythonPlugins(string directory)
        {
            string pluginDirectory = Path.Join(directory, "PythonPlugins");
            if (!Directory.Exists(pluginDirectory))
                return;
        }

        public static void LoadBaseRegistryObjects(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsAbstract && type.CustomAttributes.Any(a => a.AttributeType == typeof(BaseRegistryObjectAttribute)))
                {
                    Registry.registry[type] = new();
                }
            }
        }
    }

    public enum LoadStage
    {
        OnLoad,
        PostLoadTypes,
        PostLoadPlugins
    }
}