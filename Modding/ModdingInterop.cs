using System;
using System.Linq;
using Edelweiss.Interop;

namespace Edelweiss.Modding
{
    public class ModdingInterop : PluginInterop
    {
        public void ChangeMapPreset(string name)
        {
            if(!ModdingTab.MapPresets.Value.ContainsKey(name))
            {
                ModdingTab.MapPresets[name] = [new MapDirectory()];
            }
            ModdingTab.CurrentPresetName.Value = name;
        }

        public void RemoveMapPreset(string name)
        {
            ModdingTab.MapPresets.Remove(name);
        }

        public void RenameMapPreset(string prev, string name)
        {
            if(ModdingTab.MapPresets.Value.ContainsKey(name))
                return;

            using(ModdingTab.MapPresets.Suppress())
            {
                ModdingTab.MapPresets[name] = ModdingTab.MapPresets[prev];
                ModdingTab.MapPresets.Remove(prev);
            }
        }
    }
}