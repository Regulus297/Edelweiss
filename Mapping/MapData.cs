using Edelweiss.Interop;
using Edelweiss.Mapping.Entities;
using Edelweiss.Modding;

namespace Edelweiss.Mapping
{
    public class MapData
    {
        public BindableVariable<string> Name = "";
        public BindableVariable<string> MapType = "";
        public BindableVariable<MapDirectory> MapDirectory = new BindableVariable<MapDirectory>(null);
        public BindableVariable<MapMeta> MapMeta = new MapMeta();

        public MapData()
        {
            MapType.ValueChanged += value => MapDirectory.Value = ModdingTab.CurrentMod.Value.MapHierarchy.Value.Find(d => d.Name.Value == value);
        }
    }
}