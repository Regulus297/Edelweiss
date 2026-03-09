using System;
using System.Linq;
using Edelweiss.Interop;

namespace Edelweiss.Modding
{
    /// <summary>
    /// Interop for the modding tab
    /// </summary>
    public class ModdingInterop : PluginInterop
    {
        /// <summary>
        /// Changes the current map preset, adds a new one if it doesn't already exist
        /// </summary>
        public void ChangeMapPreset(string name)
        {
            if(!ModdingTab.MapPresets.Value.ContainsKey(name))
            {
                ModdingTab.MapPresets[name] = [new MapDirectory()];
            }
            ModdingTab.CurrentPresetName.Value = name;
        }

        /// <summary>
        /// Removes the given map preset
        /// </summary>
        public void RemoveMapPreset(string name)
        {
            ModdingTab.MapPresets.Remove(name);
        }

        /// <summary>
        /// Renames the given map preset
        /// </summary>
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