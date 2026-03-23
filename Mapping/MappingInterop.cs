using System;
using Edelweiss.Interop;
using Edelweiss.Utils;

namespace Edelweiss.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    public class MappingInterop: PluginInterop
    {
        public void CreateMap()
        {
            MappingTab.CreatingMap.Value = new MapData();
            UI.OpenPopupWidget("Edelweiss:Forms/map_creation");
        }
    }
}