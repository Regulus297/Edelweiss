using System;
using System.ComponentModel;
using System.IO;
using Edelweiss.Interop;
using Edelweiss.Mapping;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Edelweiss.Modding
{
    /// <summary>
    /// Class containing the data for a Celeste mod
    /// </summary>
    public class ModData
    {
        /// <summary>
        /// The name of the mod
        /// </summary>
        [JsonIgnore]
        public BindableVariable<string> Name;

        /// <summary>
        /// The name of the mapper
        /// </summary>
        public BindableVariable<string> Mapper;

        /// <summary>
        /// The root directory which the mod will be placed in
        /// </summary>
        [JsonIgnore]
        public BindableVariable<string> RootDirectory = Path.Join(MainPlugin.CelesteDirectory, "Mods");

        /// <summary>
        /// The map hierarchy used 
        /// </summary>
        public BindableList<MapDirectory> MapHierarchy
        {
            get => mapHierarchy;
            set => mapHierarchy.Value = value.Value;
        }

        private BindableList<MapDirectory> mapHierarchy = [];

        /// <summary>
        /// The names of all map directory presets in this mod
        /// </summary>
        public BindableList<string> MapDirectoryNames = [];

        /// <summary>
        /// A dictionary of each of the map types to the maps of that type
        /// </summary>
        public BindableDictionary<string, BindableList<string>> MapsByType = [];

        /// <summary>
        /// All maps in this mod
        /// </summary>
        public BindableList<MapData> Maps = [];

        /// <summary>
        /// This is not saved or set when loading from disk, only for setting the hierarchy when creating a new mod. Do not use.
        /// </summary>
        [JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public BindableVariable<string> MapHierarchyName = "";

        /// <summary>
        /// 
        /// </summary>
        public ModData()
        {
            MapHierarchyName.ValueChanged += value => MapHierarchy.Value = ModdingTab.MapPresets[value];
            MapDirectoryNames.MakeTransform(MapHierarchy, d => d.Name.Value);
            MapDirectoryNames.ValueChanged += v =>
            {
                MapsByType.Value = [];
                foreach(string type in v)
                {
                    MapsByType[type] = [];
                }
            };
        }

        /// <summary>
        /// Saves the mod to a folder in the Celeste directory
        /// </summary>
        public void Save()
        {
            string modDirectory = Path.Join(RootDirectory.Value, Name.Value).Replace('/', Path.DirectorySeparatorChar);
            Directory.CreateDirectory(modDirectory);

            // everest.yaml
            var everestYaml = new
            {
                Name = Name.Value,
                Version = "0.0.1"
            };
            var yamlList = new[] {everestYaml};
            ISerializer serializer = new SerializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();
            File.WriteAllText(Path.Join(modDirectory, "everest.yaml"), serializer.Serialize(yamlList));

            // Edelweiss meta
            File.WriteAllText(Path.Join(modDirectory, "edelweiss.meta.json"), JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        /// <summary>
        /// Loads a mod from the given path. Only works if the folder given contains edelweiss.meta.json
        /// </summary>
        public static ModData Load(string path)
        {
            if(File.Exists(Path.Join(path, "edelweiss.meta.json")))
            {
                ModData data = JsonConvert.DeserializeObject<ModData>(File.ReadAllText(Path.Join(path, "edelweiss.meta.json")));
                data.ModDirectory = path;
                return data;
            }
            return null;
        }

        /// <summary>
        /// Add a map to the mod
        /// </summary>
        public void AddMap(MapData map)
        {
            MapsByType[map.MapDirectory.Value.Name.Value].Add(map.Name.Value);
            Maps.Add(map);
        }

        /// <summary>
        /// The path to the mod folder
        /// </summary>
        public string ModDirectory
        {
            get => Path.Join(RootDirectory.Value, Name.Value);
            set
            {
                Name = value.Split(Path.DirectorySeparatorChar)[^1];
                RootDirectory = string.Join(Path.DirectorySeparatorChar, value.Split(Path.DirectorySeparatorChar)[..^1]);
            }
        }
    }
}