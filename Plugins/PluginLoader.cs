using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Edelweiss.Interop;
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

        internal static Dictionary<string, TextureData> textureDataCache = [];
        internal static List<string> vanillaTextures = [];

        private static List<string> blacklist = [];

        /// <summary>
        /// Invoked after all types in a plugin are loaded
        /// </summary>
        public static event Action PostLoadTypes;

        /// <summary>
        /// Invoked after all plugins are loaded
        /// </summary>
        public static event Action PostLoadPlugins;

        /// <summary>
        /// Dictionary containing language keys to localization dictionary.
        /// </summary>
        public static Dictionary<string, Dictionary<string, string>> localization = [];
        

        internal static void LoadPlugins()
        {
            LoadBlacklist();
            LoadLangFiles(Directory.GetCurrentDirectory(), "Edelweiss");
            LoadPythonPlugins(Directory.GetCurrentDirectory(), "Edelweiss");
            LoadAssembly(Assembly.GetExecutingAssembly());
            LoadJsonFiles(Directory.GetCurrentDirectory(), "Edelweiss");
            LoadTexturesFromDirectory(Path.Join(Directory.GetCurrentDirectory(), "Resources"));

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
                        continue;
                    }

                    LoadLangFiles(pluginAsset, plugin.ID);
                    LoadPythonPlugins(pluginAsset, plugin.ID);
                    LoadJsonFiles(pluginAsset, plugin.ID);
                }
            }

            Registry.ForAll<Plugin>(plugin => plugin.PostSetupContent());
            PostLoadPlugins?.Invoke();
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
            PostLoadTypes?.Invoke();
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
        public static void LoadPythonPlugins(PluginAsset directory, string pluginID)
        {
            string pluginDirectory = "PythonPlugins";
            if (!directory.DirExists(pluginDirectory))
                return;

            var files = directory.GetFiles(pluginDirectory, "*.py", SearchOption.AllDirectories);
            if (files.Length == 0)
                return;
            
            foreach(string file in files)
            {
                string key = $"{pluginID}:{file[(pluginDirectory.Length + 1)..^3]}";
                key = key.Replace(Path.DirectorySeparatorChar, '/');
                MainVars.PythonPlugins[key] = file;
            }
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
                string key = $"{pluginID}:{file[(jsonDirectory.Length + 1)..^5]}";
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
            string language = key.Split(Path.DirectorySeparatorChar)[0];
            if (!localization.ContainsKey(language))
                localization[language] = [];

            key = string.Join('.', key.Split(Path.DirectorySeparatorChar)[1..]);
            if(key.EndsWith("._"))
            {
                key = key[..^2];
            }

            string line;
            using (StreamReader reader = new(fileContent))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith('#') || !line.Contains('='))
                        continue;

                    localization[language][pluginID + "." + key + "." + line.Split("=", StringSplitOptions.TrimEntries)[0]] = line.Split("=", StringSplitOptions.TrimEntries)[1];
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
                if (type.IsAbstract && type.CustomAttributes.Any(a => a.AttributeType == typeof(BaseRegistryObjectAttribute)))
                {
                    Registry.registry[type] = new();
                }
            }
        }

        internal static bool LoadTexturesFromDirectory(PluginAsset path)
        {
            string graphicsPath = Path.Join("Graphics", "Atlases");
            if (!path.DirExists(graphicsPath))
                return false;

            Logger.Log(nameof(PluginLoader), $"Loading textures from {path}");

            foreach (string file in path.GetFiles(graphicsPath, "*.png", SearchOption.AllDirectories))
            {
                string key = file.Substring(0, file.Length - 4).Substring(graphicsPath.Length + 1).Replace(Path.DirectorySeparatorChar, '/');
                if (vanillaTextures.Contains(key))
                {
                    Logger.Warn(nameof(PluginLoader), $"Mod {path} replaces vanilla texture {key}: skipping the texture");
                    continue;
                }
                MainVars.TexturePaths[key] = path.IsZipFile ? path.PluginArchive.Path() + char.ConvertFromUtf32(0) + file : Path.Join(path.AssetPath, file);
            }

            return true;
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

    /// <summary>
    /// The stages of plugin loading
    /// </summary>
    public enum LoadStage
    {
        /// <summary>
        /// When the type itself is loaded
        /// </summary>
        OnLoad,
        /// <summary>
        /// After all types in the plugin are loaded
        /// </summary>
        PostLoadTypes,
        /// <summary>
        /// After all plugins are loaded
        /// </summary>
        PostLoadPlugins
    }

    /// <summary>
    /// Contains data for a texture
    /// </summary>
    public class TextureData
    {
        private int _width;

        /// <summary>
        /// The width of the texture in pixels
        /// </summary>
        public int width
        {
            get
            {
                return atlasWidth < 0 ? _width : atlasWidth;
            }
            set
            {
                _width = value;
            }
        }

        private int _height;

        /// <summary>
        /// The height of the texture in pixels
        /// </summary>
        public int height
        {
            get
            {
                return atlasHeight < 0 ? _height : atlasHeight;
            }
            set
            {
                _height = value;
            }
        }

        /// <summary>
        /// The position of the texture in the atlas
        /// </summary>
        public int atlasX = -1, atlasY = -1;

        /// <summary>
        /// The width and height of the texture in the atlas
        /// </summary>
        public int atlasWidth = -1, atlasHeight = -1;

        /// <summary>
        /// The position at which the texture starts in the atlas as compared to the padded texture
        /// </summary>
        public int atlasOffsetX = 0, atlasOffsetY = 0;

        private int _paddedWidth = 0, _paddedHeight = 0;

        /// <summary>
        /// The width of the padded texture
        /// </summary>
        public int paddedWidth
        {
            get
            {
                return _paddedWidth <= 0 ? width : _paddedWidth;
            }
            set
            {
                _paddedWidth = value;
            }
        }

        /// <summary>
        /// The height of the padded texture
        /// </summary>
        public int paddedHeight
        {
            get
            {
                return _paddedHeight <= 0 ? height : _paddedHeight;
            }
            set
            {
                _paddedHeight = value;
            }
        }
    }
}