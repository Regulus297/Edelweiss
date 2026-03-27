using System;
using Edelweiss.Interop;
using Edelweiss.Modding;
using Edelweiss.Utils;

namespace Edelweiss.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    public class MappingInterop: PluginInterop
    {
        /// <summary>
        /// Opens a popup to create the given map
        /// </summary>
        public void CreateMap()
        {
            MappingTab.CreatingMap.CopyFrom(new MapData());
            UI.OpenPopupWidget("Edelweiss:Forms/map_creation");
        }

        /// <summary>
        /// Saves the currently being created map and sets it to the current map.
        /// </summary>
        public void SaveMap()
        {
            MappingTab.CurrentMap.CopyFrom(MappingTab.CreatingMap);
            ModdingTab.CurrentMod.Value.AddMap(MappingTab.CreatingMap);
        }
    }
}