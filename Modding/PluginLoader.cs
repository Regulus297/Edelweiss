using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using Edelweiss.Network;
using Edelweiss.RegistryTypes;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Plugins
{
    public static class PluginLoader
    {
        public static Dictionary<string, string> jsonPaths = [];
        public static Dictionary<string, string> jsonCache = [];
        public static Dictionary<string, Dictionary<string, string>> localization = [];

        public static void LoadPlugins()
        {
            LoadLangFiles(Directory.GetCurrentDirectory(), "Edelweiss");
            LoadPythonPlugins(Directory.GetCurrentDirectory());
            LoadAssembly(Assembly.GetExecutingAssembly());
            LoadJsonFiles(Directory.GetCurrentDirectory(), "Edelweiss");

            string directory = Path.Join(Directory.GetCurrentDirectory(), "Plugins");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);


            foreach (string modDirectory in Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly))
            {
                if (ValidateModDirectory(modDirectory, out string modFilePath))
                {
                    Assembly assembly = Assembly.LoadFile(modFilePath);
                    Plugin plugin = LoadAssembly(assembly);
                    LoadLangFiles(Directory.GetCurrentDirectory(), plugin.ID);
                    LoadPythonPlugins(directory);
                    LoadJsonFiles(directory, plugin.ID);
                }
            }

            Registry.ForAll<Plugin>(plugin => plugin.PostLoad());
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
                    plugin.Logger.Log($"Loading plugin {plugin.ID}");
                }
            }
            
            LoadTypes(plugin, assembly.GetTypes(), out var failed);
            int failedCount = 0;
            while (failed.Count > 0 && failedCount != failed.Count)
            {
                failedCount = failed.Count;
                LoadTypes(plugin, failed, out var temp);
                failed = temp;
            }

            // If any classes still failed to load, log it
            foreach (Type type in failed)
            {
                plugin.Logger.Warn($"Failed to load {type} from {plugin.ID}");
            }
            return plugin;
        }

        private static void LoadTypes(Plugin plugin, IEnumerable<Type> types, out List<Type> failedLoading)
        {
            failedLoading = [];
            foreach (Type type in types)
            {
                foreach (Type baseType in Registry.registry.Keys)
                {
                    if (!type.IsAbstract && type.GetInterfaces().Any(i => i == typeof(IRegistryObject)) && type.IsAssignableTo(baseType) && !type.IsAssignableTo(typeof(Plugin)))
                    {
                        if (type.CustomAttributes.Any(a => a.AttributeType == typeof(LoadAfterAttribute)))
                        {
                            if (type.GetCustomAttribute<LoadAfterAttribute>().otherTypes.Any(t => !Registry.allRegisteredTypes.Contains(t)))
                            {
                                failedLoading.Add(type);
                                break;
                            }
                        }
                        IRegistryObject instance = (IRegistryObject)Activator.CreateInstance(type);
                        if (instance is PluginRegistryObject o)
                        {
                            o.Plugin = plugin;
                            plugin.OnPostSetupContent += o.PostSetupContent;
                        }

                        Registry.registry[baseType].Add(instance);
                        Registry.allRegisteredTypes.Add(type);
                        instance.OnRegister();
                        break;
                    }
                }
            }
        }

        public static void LoadPythonPlugins(string directory)
        {
            string pluginDirectory = Path.Join(directory, "PythonPlugins");
            if (!Directory.Exists(pluginDirectory))
                return;

            var files = Directory.GetFiles(pluginDirectory, "*.py", SearchOption.AllDirectories);
            if (files.Length == 0)
                return;
            JToken token = JToken.FromObject(files);
            JObject obj = new()
            {
                { "files", token }
            };
            NetworkManager.SendPacket(Netcode.REGISTER_PYTHON_PLUGINS, obj);
        }

        public static void LoadJsonFiles(string directory, string pluginID)
        {
            string jsonDirectory = Path.Join(directory, "Resources", "JSON");
            if (!Directory.Exists(jsonDirectory))
                return;

            foreach (string file in Directory.GetFiles(jsonDirectory, "*.json", SearchOption.AllDirectories))
            {
                string key = $"{pluginID}:{file.Substring(0, file.Length - 5).Substring(jsonDirectory.Length + 1)}";
                key = key.Replace(Path.DirectorySeparatorChar, '/');
                jsonPaths[key] = file;
            }
        }

        public static void LoadLangFiles(string directory, string pluginID)
        {
            string langDirectory = Path.Join(directory, "Resources", "Localization");
            if (!Directory.Exists(langDirectory))
                return;

            foreach (string file in Directory.GetFiles(langDirectory, "*.lang", SearchOption.AllDirectories))
            {
                string key = file.Substring(0, file.Length - 5).Substring(langDirectory.Length + 1);
                key = key.Replace(Path.DirectorySeparatorChar, '/');
                localization[key] = [];
                string line;
                using (StreamReader reader = new(file))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("#") || !line.Contains("="))
                            continue;

                        localization[key][pluginID + "." + line.Split("=", StringSplitOptions.TrimEntries)[0]] = line.Split("=", StringSplitOptions.TrimEntries)[1];
                    }
                }
            }
        }

        public static string RequestJson(string key)
        {
            if (!jsonPaths.TryGetValue(key, out string jsonPath))
                return "{}";

            if (jsonCache.TryGetValue(key, out string json))
                return json;

            using (StreamReader reader = new(jsonPath))
            {
                json = reader.ReadToEnd();
                jsonCache[key] = json;
                return json;
            }
        }

        public static JObject RequestJObject(string key)
        {
            return JObject.Parse(RequestJson(key));
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