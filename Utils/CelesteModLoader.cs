using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text;
using Edelweiss.Loenn;
using Edelweiss.Mapping.Entities;
using Edelweiss.Plugins;
using MoonSharp.Interpreter;

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


        internal static Dictionary<string, EntityData> entities = [];
        internal static bool LoadTexturesFromDirectory(string graphicsPath)
        {
            MainPlugin.Instance.Logger.Log($"Loading textures from {graphicsPath}");
            graphicsPath = Path.Join(graphicsPath, "Graphics", "Atlases");
            if (!Directory.Exists(graphicsPath))
                return false;

            foreach (string file in Directory.GetFiles(graphicsPath, "*.png", SearchOption.AllDirectories))
            {
                string key = file.Substring(0, file.Length - 4).Substring(graphicsPath.Length + 1).Replace(Path.DirectorySeparatorChar, '/');
                texturePaths[key] = file;
            }

            return true;
        }

        internal static void LoadMods()
        {
            foreach (string path in Directory.GetDirectories(Path.Join(MainPlugin.CelesteDirectory, "Mods")))
            {
                LoadDirectory(path);
            }
            foreach (string path in Directory.GetFiles(Path.Join(MainPlugin.CelesteDirectory, "Mods"), "*.zip"))
            {
                LoadZip(path);
            }
            MainPlugin.textures.Value = texturePaths;
        }

        internal static void LoadDirectory(string modPath)
        {
            MainPlugin.Instance.Logger.Log($"Loading mod from folder {modPath}");
            string loennEntityPath = Path.Join(modPath, "Loenn", "entities");
            string loennLangPath = Path.Join(modPath, "Loenn", "lang");

            if (Directory.Exists(loennLangPath))
            {
                PluginLoader.LoadLangFiles(loennLangPath, "Loenn", true);
            }
            if (Directory.Exists(loennEntityPath))
            {
                foreach (string entityFile in Directory.GetFiles(loennEntityPath, "*.lua", SearchOption.AllDirectories))
                {
                    Table table = LoadLua(entityFile, File.ReadAllText(entityFile), out Script script);
                    if (table != null)
                    {
                        CreateEntities(entityFile, table, script);
                    }
                }
            }
        }

        internal static void LoadZip(string modPath)
        {
            MainPlugin.Instance.Logger.Log($"Loading mod from .zip {modPath}");
            using (ZipArchive zip = ZipFile.OpenRead(modPath))
            {
                foreach (ZipArchiveEntry langEntry in zip.Entries)
                {
                    if (langEntry.FullName.StartsWith("Loenn/lang") && langEntry.FullName.EndsWith(".lang"))
                    {
                        using (var stream = langEntry.Open())
                        {
                            PluginLoader.LoadLangFile(stream, langEntry.Name, "Loenn");
                        }
                    }
                    if (langEntry.FullName.StartsWith("Graphics/Atlases") && langEntry.FullName.EndsWith(".png"))
                    {
                        string key = langEntry.FullName.Substring(0, langEntry.FullName.Length - 4).Substring("Graphics/Atlases".Length + 1);
                        texturePaths[key] = $"{modPath}{char.ConvertFromUtf32(0)}{langEntry.FullName}";
                    }
                }
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    if (entry.FullName.StartsWith("Loenn/entities") && entry.FullName.EndsWith(".lua"))
                    {
                        using (var stream = entry.Open())
                        {
                            using (var streamReader = new StreamReader(stream))
                            {
                                Table table = LoadLua(entry.FullName, streamReader.ReadToEnd(), out Script script);
                                if (table != null)
                                {
                                    CreateEntities(entry.FullName, table, script);
                                }
                            }
                        }
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
                foreach (var pair in LoennModule.globalModules)
                {
                    tempScript.Globals[pair.Key] = pair.Value(tempScript);
                }
                script = tempScript;
                return script.DoString(lua).Table;
            }
            catch (ScriptRuntimeException e)
            {
                MainPlugin.Instance.Logger.Error($"Failed loading entity {fileName} with error: \n {e.Message}");
            }
            catch (Exception e)
            {
                MainPlugin.Instance.Logger.Error($"Failed loading entity {fileName} with error: \n {LuaErrorEncoder.GetString(LuaErrorEncoder.GetBytes(e.ToString()))}");
            }
            script = tempScript;
            return null;
        }

        internal static void CreateEntities(string fileName, Table table, Script script)
        {
            try
            {
                DynValue placementValue = table.Get("placements");
                if (placementValue.Type != DataType.Table)
                    return;

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
                    LuaEntityData entityData = new(table.Get("name").String, placement.Get("name").String, placement, script, table);
                    entities[entityData.Name] = entityData;
                }
            }
            catch (Exception e)
            {
                MainPlugin.Instance.Logger.Error($"Failed loading entity {fileName} with error: \n {LuaErrorEncoder.GetString(LuaErrorEncoder.GetBytes(e.ToString()))}");
            }
        }

        public static TextureData GetTextureData(string key)
        {
            if (textureDataCache.TryGetValue(key, out var data))
                return data;

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

        public static Size GetImageSize(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            br.ReadBytes(16);
            byte[] widthArray = new byte[sizeof(int)];
            for (int i = 0; i < widthArray.Length; i++)
                widthArray[widthArray.Length - 1 - i ] = br.ReadByte();
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
        /// <summary>
        /// The width of the texture in pixels
        /// </summary>
        public int width;

        /// <summary>
        /// The height of the texture in pixels
        /// </summary>
        public int height;

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