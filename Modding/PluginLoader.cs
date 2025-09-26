using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text.Json.Nodes;
using Edelweiss.Network;
using Edelweiss.RegistryTypes;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Plugins
{
    /// <summary>
    /// Handles all plugin-related code.
    /// </summary>
    public static class PluginLoader
    {
        private static Dictionary<string, PluginFileData> jsonPaths = [];
        private static Dictionary<string, string> jsonCache = [];

        private static List<string> blacklist = [];

        /// <summary>
        /// Dictionary containing language keys to localization dictionary.
        /// </summary>
        public static Dictionary<string, Dictionary<string, string>> localization = [];
        

        internal static void LoadPlugins()
        {
            LoadBlacklist();
            LoadLangFiles(Directory.GetCurrentDirectory(), "Edelweiss");
            LoadPythonPlugins(Directory.GetCurrentDirectory());
            LoadAssembly(Assembly.GetExecutingAssembly());
            LoadJsonFiles(Directory.GetCurrentDirectory(), "Edelweiss");
            CelesteModLoader.LoadTexturesFromDirectory(Path.Join(Directory.GetCurrentDirectory(), "Resources"));

            string directory = Path.Join(Directory.GetCurrentDirectory(), "Plugins");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            foreach (PluginAsset pluginAsset in EdelweissUtils.GetPluginAssetsFromDirectory(directory))
            {
                if (ValidateModDirectory(pluginAsset, out string modFilePath))
                {
                    Assembly assembly;
                    // Deflate stream first
                    using (Stream stream = pluginAsset.GetStream(modFilePath))
                    {
                        MemoryStream ms = new();    
                        stream.CopyTo(ms);
                        ms.Position = 0;
                        assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                    }
                    Plugin plugin = LoadAssembly(assembly);
                    if (plugin == null)
                    {
                        // Logger.Error("Edelweiss", $"Assembly {modFilePath} does not define a Plugin class, skipping loading");
                        continue;
                    }

                    LoadLangFiles(pluginAsset, plugin.ID);
                    LoadPythonPlugins(pluginAsset);
                    LoadJsonFiles(pluginAsset, plugin.ID);
                    CelesteModLoader.LoadTexturesFromDirectory(pluginAsset);
                }
            }

            Registry.ForAll<Plugin>(plugin => plugin.PostLoad());
        }

        internal static void LoadBlacklist()
        {
            if (!File.Exists("blacklist.txt"))
                File.Open("blacklist.txt", FileMode.CreateNew);
            blacklist = File.ReadAllLines("blacklist.txt").Select(t => t.Trim()).ToList();
        }

        private static bool ValidateModDirectory(PluginAsset modDirectory, out string modFilePath)
        {
            modFilePath = "";
            string[] files = modDirectory.GetFiles("*.dll");
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

        /// <summary>
        /// Loads all the types from an assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns>The plugin that the assembly defines</returns>
        public static Plugin LoadAssembly(Assembly assembly)
        {
            LoadBaseRegistryObjects(assembly);
            Plugin plugin = null;
            foreach (Type type in assembly.GetTypes())
            {
                if (!type.IsAbstract && type.IsAssignableTo(typeof(Plugin)))
                {
                    plugin = (Plugin)Activator.CreateInstance(type);
                    if (blacklist.Contains(plugin.ID))
                        return null;
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

        /// <summary>
        /// Finds all the python plugins from a given directory and registers them with the frontend.
        /// </summary>
        public static void LoadPythonPlugins(PluginAsset directory)
        {
            string pluginDirectory = "PythonPlugins";
            if (!directory.DirExists(pluginDirectory))
                return;

            var files = directory.GetFiles(pluginDirectory, "*.py", SearchOption.AllDirectories);
            if (files.Length == 0)
                return;
            JToken token = JToken.FromObject(files);
            JObject obj = new()
            {
                { "files", token }
            };
            NetworkManager.SendPacket(Netcode.REGISTER_PYTHON_PLUGINS, obj);
        }

        /// <summary>
        /// Loads all the JSON files from a given directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="pluginID">The prefix that will be added to the key</param>
        public static void LoadJsonFiles(PluginAsset directory, string pluginID)
        {
            string jsonDirectory = Path.Join("Resources", "JSON");
            if (!directory.DirExists(jsonDirectory))
                return;

            foreach (string file in directory.GetFiles(jsonDirectory, "*.json", SearchOption.AllDirectories))
            {
                string key = $"{pluginID}:{file.Substring(0, file.Length - 5).Substring(jsonDirectory.Length + 1)}";
                key = key.Replace(Path.DirectorySeparatorChar, '/');
                jsonPaths[key] = new(file, directory);
            }
        }

        /// <summary>
        /// Loads the language files from a given directory.
        /// </summary>
        /// <param name="directory">The directory to load from</param>
        /// <param name="pluginID">The prefix that will be added to the file key</param>
        /// <param name="celesteMod">True if the directory is a Celeste mod, false if it is an Edelweiss plugin</param>
        public static void LoadLangFiles(PluginAsset directory, string pluginID, bool celesteMod = false)
        {
            string langDirectory = celesteMod ? Path.Join("Loenn", "lang") : Path.Join("Resources", "Localization");
            if (!directory.DirExists(langDirectory))
                return;

            foreach (string file in directory.GetFiles(langDirectory, "*.lang", SearchOption.AllDirectories))
            {
                using Stream fileContent = directory.GetStream(file);
                LoadLangFile(fileContent, file.Substring(langDirectory.Length + 1), pluginID);
            }
        }

        internal static void LoadLangFile(Stream fileContent, string file, string pluginID)
        {
            string key = file.Substring(0, file.Length - 5);
            if (!localization.ContainsKey(key))
                localization[key] = [];

            string line;
            using (StreamReader reader = new(fileContent))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#") || !line.Contains("="))
                        continue;

                    localization[key][pluginID + "." + line.Split("=", StringSplitOptions.TrimEntries)[0]] = line.Split("=", StringSplitOptions.TrimEntries)[1];
                }
            }
        }

        /// <summary>
        /// Returns the JSON content for the given key.
        /// </summary>
        /// <returns>An empty JSON object as a string if the key does not exist.</returns>
        public static string RequestJson(string key)
        {
            if (!jsonPaths.TryGetValue(key, out PluginFileData jsonPath))
                return "{}";

            if (jsonCache.TryGetValue(key, out string json))
                return json;

            using (StreamReader reader = new(jsonPath.asset.GetStream(jsonPath.filePath)))
            {
                json = reader.ReadToEnd();
                jsonCache[key] = json;
                return json;
            }
        }

        /// <summary>
        /// Requests the JSON for a given key and parses it.
        /// </summary>
        public static JObject RequestJObject(string key)
        {
            return JObject.Parse(RequestJson(key));
        }

        /// <summary>
        /// Loads the base registry types for a given assembly.
        /// </summary>
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

    /// <summary>
    /// Contains the plugin asset for any given file and the path relative to the plugin asset
    /// </summary>
    public struct PluginFileData(string filePath, PluginAsset asset)
    {
        /// <summary>
        /// The path of the file relative to the plugin asset
        /// </summary>
        public string filePath = filePath;

        /// <summary>
        /// The plugin asset that contains the file
        /// </summary>
        public PluginAsset asset = asset;
    }
}