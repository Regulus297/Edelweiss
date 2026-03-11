using System;
using System.Collections.Generic;
using Edelweiss.Interop;
using Edelweiss.Plugins;

namespace Edelweiss.Modding
{
    /// <summary>
    /// 
    /// </summary>
    public class ModdingTab : CustomTab, ISyncable
    {
        /// <inheritdoc/>
        public override string LayoutJSON => "Edelweiss:mod_tab";

        /// <inheritdoc/>
        public override string ToolbarJSON => "Edelweiss:mod_toolbar";

        /// <inheritdoc/>
        public override string DisplayName => "Modding";

        /// <summary>
        /// The list of all map presets
        /// </summary>
        public static BindableDictionary<string, List<MapDirectory>> MapPresets = [];

        /// <summary>
        /// The currently selected preset
        /// </summary>
        public static BindableList<MapDirectory> CurrentPreset = [new MapDirectory()];

        /// <summary>
        /// The names of all map presets
        /// </summary>
        public static BindableList<string> MapPresetNames = [];

        /// <summary>
        /// The name of the currently selected preset
        /// </summary>
        public static BindableVariable<string> CurrentPresetName;

        /// <summary>
        /// The mod that is currently being created
        /// </summary>
        public static BindableVariable<ModData> CreatingMod;

        string ISyncable.Name() => FullName;

        /// <inheritdoc/>
        public override void Load()
        {
            CurrentPresetName = "Default";
            MapPresetNames.MakeTransform(MapPresets, (name, _) => name);
            CurrentPresetName.ValueChanged += value => CurrentPreset.Value = MapPresets[value];
            CurrentPresetName.Value = "Default";
            base.Load();
        }
    }
}