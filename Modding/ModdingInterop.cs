using System;
using System.Linq;
using Edelweiss.Interop;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

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
            using(ModdingTab.MapPresets.Suppress()) {
                if(!ModdingTab.MapPresets.Value.ContainsKey(name))
                {
                    ModdingTab.MapPresets[name] = [new MapDirectory()];
                }
                ModdingTab.CurrentPresetName.Value = name;
            }
        }

        /// <summary>
        /// Removes the given map preset
        /// </summary>
        public void RemoveMapPreset(string name)
        {
            using(ModdingTab.MapPresets.Suppress()) {
                ModdingTab.MapPresets.Remove(name);
            }
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

        /// <summary>
        /// Opens a popup form for mod creation
        /// </summary>
        public void CreateMod()
        {
            ModdingTab.CreatingMod = new ModData()
            {
                Name = "",
                Mapper = ""
            };
            UI.OpenPopupWidget("Edelweiss:Forms/mod_creation");
        }

        /// <summary>
        /// Saves the current mod to the Celeste mods directory
        /// </summary>
        public void SaveMod()
        {
            ModdingTab.CreatingMod.Value.Save();
        }
    }
}