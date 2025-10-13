using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edelweiss.Loenn;
using Edelweiss.Mapping.Entities;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Edelweiss.Preferences;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;
using YamlDotNet.RepresentationModel;

namespace Edelweiss.Utils
{
    /// <summary>
    /// The class responsible for loading Celeste mods
    /// </summary>
    public static class CelesteModLoader
    {
        private static readonly Encoding LuaErrorEncoder = Encoding.GetEncoding(
            "UTF-8",
            new EncoderReplacementFallback(string.Empty),
            new DecoderExceptionFallback()
        );

        /// <summary>
        /// The loaded texture keys to the absolute paths to the texture
        /// </summary>
        public static Dictionary<string, string> texturePaths = [];
        internal static Dictionary<string, TextureData> textureDataCache = [];
        internal static List<string> vanillaTextures = [];
        
        /// <summary>
        /// Called after all mods are loaded on the background thread
        /// </summary>
        public static event Action PostLoadMods;


        /// <summary>
        /// The list of all loaded entities
        /// </summary>
        public static ConcurrentDictionary<string, EntityData> entities = [];

        internal static ConcurrentDictionary<string, List<string>> modEntities = [];
        internal static ConcurrentQueue<string> entityMods = [];

        /// <summary>
        /// Contains the name of the default (first) placement (entity_name.placement_name) for a given entity (entity_name)
        /// </summary>
        public static ConcurrentDictionary<string, string> defaultPlacements = [];
        internal static bool LoadTexturesFromDirectory(PluginAsset path)
        {
            string graphicsPath = Path.Join("Graphics", "Atlases");
            if (!path.DirExists(graphicsPath))
                return false;

            Logger.Log(nameof(CelesteModLoader), $"Loading textures from {path}");

            foreach (string file in path.GetFiles(graphicsPath, "*.png", SearchOption.AllDirectories))
            {
                string key = file.Substring(0, file.Length - 4).Substring(graphicsPath.Length + 1).Replace(Path.DirectorySeparatorChar, '/');
                if (vanillaTextures.Contains(key))
                {
                    Logger.Warn(nameof(CelesteModLoader), $"Mod {path} replaces vanilla texture {key}: skipping the texture");
                    continue;
                }
                texturePaths[key] = path.IsZipFile ? path.PluginArchive.Path() + char.ConvertFromUtf32(0) + file : Path.Join(path.AssetPath, file);
            }

            return true;
        }

        internal static void LoadMods()
        {
            NetworkManager.SendPacket(Netcode.MODIFY_LOADING_SCREEN, new JObject()
            {
                {"action", "open"},
                {"name", "main"}
            });
            List<PluginAsset> mods = EdelweissUtils.GetPluginAssetsFromDirectory(Path.Join(MainPlugin.CelesteDirectory, "Mods"));
            int i = 0;
            foreach (PluginAsset path in mods)
            {
                LoadMod(path);
                i++;
                NetworkManager.SendPacket(Netcode.MODIFY_LOADING_SCREEN, new JObject()
                {
                    {"action", "modify"},
                    {"progress", (float)i / mods.Count}
                });
            }
            MainPlugin.textures.Value = texturePaths;
            PostLoadMods?.Invoke();
            NetworkManager.SendPacket(Netcode.MODIFY_LOADING_SCREEN, new JObject()
            {
                {"action", "close"}
            });
        }

        internal static void LoadMod(PluginAsset modPath)
        {
            Logger.Log(nameof(CelesteModLoader), $"Loading mod from folder {modPath}");
            LoadTexturesFromDirectory(modPath);

            Stream s = modPath.GetStream("everest.yaml");
            string modName = Path.GetFileNameWithoutExtension(modPath.ToString());
            if (s != null)
            {
                using StreamReader r = new StreamReader(s);
                string yaml = r.ReadToEnd();
                YamlStream stream = new YamlStream();
                stream.Load(new StringReader(yaml));
                YamlNode root = stream.Documents[0].RootNode;
                try
                {
                    modName = root[0]["Name"]?.ToString();
                }
                catch (ArgumentException)
                {
                    
                }
            }
            s?.Dispose();

            string loennEntityPath = Path.Join("Loenn", "entities");
            string loennLangPath = Path.Join("Loenn", "lang");

            if (modPath.DirExists(loennLangPath))
            {
                PluginLoader.LoadLangFiles(modPath, "Loenn", true);
            }
            if (modPath.DirExists(loennEntityPath))
            {
                foreach (string entityFile in modPath.GetFiles(loennEntityPath, "*.lua", SearchOption.AllDirectories))
                {
                    using StreamReader reader = new(modPath.GetStream(entityFile));
                    Table table = LoadLua(entityFile.Split("/")[^1], reader.ReadToEnd(), out Script script);
                    if (table != null)
                    {
                        CreateEntities(modName, entityFile, table, script);
                    }
                }
            }
        }

        internal static Table LoadLua(string fileName, string lua, out Script script)
        {
            Script tempScript = new();
            try
            {
                tempScript.Globals["require"] = (Func<string, DynValue>)(module => LoennModule.RequireModule(tempScript, module));

                // Required by aonHelper's Darker Matter
                // It draws in Loenn so I assume it [Loenn] defines a split method but I can't be fucked finding where
                tempScript.DoString(@"
                function string:split(sep)
                    local result = {}
                    local pattern = string.format('([^%s]+)', sep)
                    for part in self:gmatch(pattern) do
                        table.insert(result, part)
                    end
                    return result
                end
                ");


                foreach (var pair in LoennModule.globalModules)
                {
                    tempScript.Globals[pair.Key] = pair.Value(tempScript);
                }
                script = tempScript;
                return script.DoString(lua).Table;
            }
            catch (ScriptRuntimeException e)
            {
                Logger.Error(nameof(CelesteModLoader), $"Failed loading entity {fileName} with error: \n {e.Message}");
            }
            catch (Exception e)
            {
                Logger.Error(nameof(CelesteModLoader), $"Failed loading entity {fileName} with error: \n {LuaErrorEncoder.GetString(LuaErrorEncoder.GetBytes(e.ToString()))}");
            }
            script = tempScript;
            return null;
        }

        internal static void CreateEntities(string modName, string fileName, Table table, Script script)
        {
            try
            {
                DynValue placementValue = table.Get("placements");
                if (placementValue.Type != DataType.Table)
                {
                    // It's a list of entities
                    foreach (DynValue value in table.Values)
                    {
                        CreateEntities(modName, fileName, value.Table, script);
                    }
                }

                List<Table> placements = [placementValue.Table];
                if (placementValue.Table.Get("name").IsNil())
                {
                    // It's a list of tables
                    placements.Clear();
                    foreach (DynValue v in placementValue.Table.Values)
                        placements.Add(v.Table);
                }

                foreach (Table placement in placements)
                {
                    LuaEntityData entityData = new(table.Get("name").String, placement.Get("name").String, placement, script, table, modName);
                    AddEntity(modName, entityData, table.Get<string>("name"));

                }
            }
            catch (Exception e)
            {
                Logger.Error(nameof(CelesteModLoader), $"Failed loading entity {fileName} with error: \n {LuaErrorEncoder.GetString(LuaErrorEncoder.GetBytes(e.ToString()))}");
            }
        }

        /// <summary>
        /// Adds an entity to the list of entities
        /// </summary>
        /// <param name="mod">The mod this entity belongs to</param>
        /// <param name="entity">The entity data</param>
        /// <param name="name">The name of the entity without the placement</param>
        public static void AddEntity(string mod, EntityData entity, string name)
        {
            entities[entity.Name] = entity;
            if (!modEntities.ContainsKey(mod))
            {
                modEntities[mod] = [];
            }
            modEntities[mod].Add(entity.Name);

            if (!entityMods.Contains(mod))
            {
                entityMods.Enqueue(mod);
            }
            if (!defaultPlacements.ContainsKey(entity.Name))
            {
                defaultPlacements[name] = entity.Name;
            }
        }

        /// <summary>
        /// Returns the data for the texture for a given key
        /// </summary>
        public static TextureData GetTextureData(string key)
        {
            if (textureDataCache.TryGetValue(key, out var data))
                return data;

            if (!texturePaths.ContainsKey(key))
                return null;

            string path = texturePaths[key];
            if (path.Contains(char.ConvertFromUtf32(0)))
            {
                string zip = path.Split(char.ConvertFromUtf32(0))[0];
                string entry = path.Split(char.ConvertFromUtf32(0))[1];
                using (ZipArchive zipArchive = ZipFile.OpenRead(zip))
                {
                    ZipArchiveEntry zipEntry = zipArchive.GetEntry(entry);
                    Size size = GetImageSize(zipEntry.Open());
                    textureDataCache[key] = new()
                    {
                        width = size.Width,
                        height = size.Height
                    };
                }
            }
            else
            {
                using (Stream stream = File.OpenRead(path))
                {
                    Size size = GetImageSize(stream);
                    textureDataCache[key] = new()
                    {
                        width = size.Width,
                        height = size.Height
                    };
                }
            }

            return textureDataCache[key];
        }

        /// <summary>
        /// Returns the dimensions of an image.
        /// </summary>
        /// <param name="stream">The image stream</param>
        public static Size GetImageSize(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            br.ReadBytes(16);
            byte[] widthArray = new byte[sizeof(int)];
            for (int i = 0; i < widthArray.Length; i++)
                widthArray[widthArray.Length - 1 - i] = br.ReadByte();
            int width = BitConverter.ToInt32(widthArray, 0);
            byte[] heightArray = new byte[sizeof(int)];
            for (int i = 0; i < heightArray.Length; i++)
                heightArray[heightArray.Length - 1 - i] = br.ReadByte();
            int height = BitConverter.ToInt32(heightArray, 0);
            return new Size(width, height);
        }
    }

    /// <summary>
    /// Contains data for a texture
    /// </summary>
    public class TextureData : ILuaConvertible
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

        /// <summary>
        /// 
        /// </summary>
        public Table ToLuaTable(Script script)
        {
            Table table = new(script);
            table["width"] = width;
            table["height"] = height;
            return table;
        }
    }
}