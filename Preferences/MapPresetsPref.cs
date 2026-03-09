using System.Collections.Generic;
using Edelweiss.Interop;
using Edelweiss.Modding;
using Edelweiss.Plugins;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Preferences
{
    public class MapPresetsPref : PluginSaveablePreference
    {
        private readonly JsonSerializerSettings settings = new JsonSerializerSettings();

        public MapPresetsPref()
        {
            settings.Converters.Add(new BindableConverter());
        }

        public override JToken Value
        {
            get => base.Value;
            set
            {
                base.Value = value;
                ModdingTab.MapPresets.Value = JsonConvert.DeserializeObject<Dictionary<string, List<MapDirectory>>>(value.ToString(), settings);
            }
        }

        public override LoadStage DefaultValueStage => LoadStage.OnLoad;

        public override void SetDefaultValue()
        {
            
            ModdingTab.MapPresets["Default"] =
            [
                new MapDirectory()
            ];
            ModdingTab.MapPresets["CollabUtils2"] =
            [
                new MapDirectory()
                {
                    Name = "Default",
                    Directory = "{mod}/{Lobby}"
                },
                new MapDirectory()
                {
                    Name = "Lobby",
                    Directory = "{mod}/0-Lobbies"
                },
                new MapDirectory()
                {
                    Name = "Gym",
                    Directory = "{mod}/0-Gyms"
                }
            ];
        }

        public override void PrepForSave()
        {
            JsonSerializer serializer = JsonSerializer.Create(settings);
            Value = JObject.FromObject(ModdingTab.MapPresets.Value, serializer);
        }
    }
}