using System;
using System.Collections.Generic;
using Edelweiss.Interop;
using Edelweiss.Plugins;

namespace Edelweiss.Modding
{
    public class ModdingTab : CustomTab, ISyncable
    {
        public override string LayoutJSON => "Edelweiss:mod_tab";

        public override string ToolbarJSON => "";

        public override string DisplayName => "Modding";

        public static BindableDictionary<string, List<MapDirectory>> MapPresets = [];
        public static BindableList<MapDirectory> CurrentPreset = [new MapDirectory()];
        public static BindableList<string> MapPresetNames = [];
        public static BindableVariable<string> CurrentPresetName;

        string ISyncable.Name() => FullName;
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