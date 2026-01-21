using System;
using System.Collections.Generic;
using System.Linq;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Edelweiss.Preferences;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.ModManagement
{
    internal class ModTab : CustomTab
    {
        public override string LayoutJSON => "Edelweiss:mod_tab";
        public override string ToolbarJSON => "Edelweiss:mod_toolbar";
        public override string DisplayName => "Mod";

        public static JObject DefaultMapPresets;
        public static SyncedVariable CustomMapPresets = new SyncedVariable("Edelweiss:CustomMapPresets");
        public static SyncedVariable AllMapPresets = new SyncedVariable("Edelweiss:AllMapPresets");

        public static long CreateModNetcode { get; private set; }
        public static long PresetChangeNetcode { get; private set; }

        public override void Load()
        {
            CreateModNetcode = Plugin.CreateNetcode("CreateMod", false);
            PresetChangeNetcode = Plugin.CreateNetcode("PresetChange", false);
        }

        public override void PostSetupContent()
        {
            DefaultMapPresets = PluginLoader.RequestJObject("Edelweiss:Misc/default_map_presets");
            RefreshPresets();
            base.PostSetupContent();
            // TODO: somehow integrate these forms into the source json
            UI.AddFormToWidget("Edelweiss:Forms/mapping_file_preset", "Modding/MappingFilePreset", enabled: false);
            UI.AddFormToWidget("Edelweiss:Forms/resource_file_preset", "Modding/ResourceFilePreset", enabled: false);
        }

        public override void HandleToolbarClick(string actionName, JObject extraData)
        {
            switch(actionName) {
                case "modMenu/createMod":
                    UI.OpenForm("Edelweiss:Forms/mod_creation");
                    break;
            }
        }

        internal static void RefreshPresets()
        {
            List<string> presets = MapPresetsPref.CustomMapPresets.Properties().Select(p => p.Name).ToList();
            CustomMapPresets.Value = presets;
            List<string> defaults = DefaultMapPresets.Properties().Select(p => p.Name).ToList();
            defaults.AddRange(presets);
            AllMapPresets.Value = defaults;
        }

        public static JObject GetMapPreset(string key)
        {
            if(DefaultMapPresets.TryGetValue(key, out JToken preset))
                return (JObject)preset;
            
            if (MapPresetsPref.CustomMapPresets.TryGetValue(key, out JToken customPreset))
                return (JObject)customPreset;
            return null;
        }
    }
}