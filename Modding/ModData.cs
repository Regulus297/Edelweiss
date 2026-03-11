using System;
using System.ComponentModel;
using System.IO;
using Edelweiss.Interop;
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
        /// The map hierarchy used 
        /// </summary>
        public BindableList<MapDirectory> MapHierarchy = [];

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
        }

        /// <summary>
        /// Saves the mod to a folder in the Celeste directory
        /// </summary>
        public void Save()
        {
            string modDirectory = Path.Join(MainPlugin.CelesteDirectory, "Mods", Name.Value);
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
    }
}