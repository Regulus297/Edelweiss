using System;
using System.Text.RegularExpressions;
using Edelweiss.Interop;
using Edelweiss.Interop.Forms;
using Edelweiss.Mapping.Entities;
using Edelweiss.Modding;
using Edelweiss.Plugins;

namespace Edelweiss.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    public class MapData: FormObject
    {
        /// <summary>
        /// The name of this map
        /// </summary>
        public BindableVariable<string> Name = "";

        /// <summary>
        /// The type of map this is
        /// </summary>
        public BindableVariable<string> MapType = "";

        /// <summary>
        /// The MapDirectory in which this map will be placed. Determined automatically.
        /// </summary>
        public BindableVariable<MapDirectory> MapDirectory = new BindableVariable<MapDirectory>(null);

        /// <summary>
        /// The metadata for the map
        /// </summary>
        public BindableVariable<MapMeta> MapMeta = new MapMeta();
        
        /// <inheritdoc/>
        public override string LocalizationRoot => "Edelweiss.Mapping.CreateMap";

        /// <summary />
        public MapData(): base()
        {
            MapType.ValueChanged += value => MapDirectory.Value = ModdingTab.CurrentMod.Value.MapHierarchy.Value.Find(d => d.Name.Value == value);
            MapDirectory.ValueChanged += RefreshParams;
        }

        private void RefreshParams(MapDirectory v)
        {
            if(v == null)
                return;
                
            foreach(var item in DynamicFields)
            {
                RemoveField(item.Key);
            }
            foreach(Match match in Regex.Matches(v.Directory.Value, "{(.+?)}"))
            {
                string value = match.Groups[1].Value;
                if(value == "mapper" || value == "mod")
                    continue;

                if(ParamIsMap(value, out string name))
                {
                    AddDynamicField(CreateOptionsField(name, $"Edelweiss:ModdingTab.CurrentMod.MapsByType.@{name}"), "");
                } 
                else
                {
                    AddDynamicField(new FormField(name, null), "");
                }
            }
        }

        private bool ParamIsMap(string param, out string name)
        {
            if(param.Contains(':'))
            {
                string[] split = param.Split(':');
                name = split[1];
                if(split[0] == "map")
                    return true;
            }
            name = param;
            return false;
        }

        /// <inheritdoc/>
        public override void InitializeFields()
        {
            AddField(nameof(Name));
            Add(CreateOptionsField(nameof(MapType), "Edelweiss:ModdingTab.CurrentMod.MapDirectoryNames"));
            AddField(nameof(MapMeta), "form", PluginLoader.RequestJObject("Edelweiss:Forms/map_meta_info"));
        }

        /// <inheritdoc/>
        public override void CopyFrom(FormObject other)
        {
            if(other is MapData m)
            {
                Name.Value = m.Name.Value;
                MapType.Value = m.MapType.Value;
                MapMeta.Value = m.MapMeta.Value;
                Value = other.Value;
            }
        }
    }
}