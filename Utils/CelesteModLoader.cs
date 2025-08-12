using System;
using System.Collections.Generic;
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
        internal static Dictionary<string, EntityData> entities = [];
        internal static bool LoadTextures(string graphicsPath)
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
        }

        internal static void LoadDirectory(string modPath)
        {
            MainPlugin.Instance.Logger.Log($"Loading mod from folder {modPath}");
            string loennEntityPath = Path.Join(modPath, "Loenn", "entities");
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
                    LuaEntityData entityData = new(table.Get("name").String + placement.Get("name").String, placement, script, table);
                    entities[entityData.Name] = entityData;
                }
            }
            catch (Exception e)
            {
                MainPlugin.Instance.Logger.Error($"Failed loading entity {fileName} with error: \n {LuaErrorEncoder.GetString(LuaErrorEncoder.GetBytes(e.ToString()))}");
            }
        }
    }
}