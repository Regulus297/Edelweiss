using System;
using Edelweiss.ModManagement;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Edelweiss.Preferences;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Modding.PacketReceivers
{
    [LoadAfter(typeof(ModTab))]
    internal class PresetChangeReceiver : PacketReceiver
    {
        public override long HandledCode => ModTab.PresetChangeNetcode;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            string id = data.Value<string>("id");
            if(id == "mapPresetSelection")
            {
                if(data.ContainsKey("old"))
                {
                    string old = data.Value<string>("old");
                    string item = data.Value<string>("new");
                    MapPresetsPref.CustomMapPresets[item] = MapPresetsPref.CustomMapPresets[old];
                    MapPresetsPref.CustomMapPresets.Remove(old);
                    ModTab.RefreshPresets();
                }
                else if(data.Value<JObject>("extraData").ContainsKey("operation"))
                {
                    string item = data.Value<string>("item");
                    MapPresetsPref.CustomMapPresets.Remove(item);
                    ModTab.RefreshPresets();
                }
                else
                {
                    string item = data.Value<string>("item");
                    if(ModTab.DefaultMapPresets.TryGetValue(item, out JToken token))
                    {
                        UI.AddFormToWidget("Edelweiss:Forms/mapping_file_preset", "Modding/MappingFilePreset", (JObject)token, enabled: false);
                        return;
                    }
                    if(!MapPresetsPref.CustomMapPresets.ContainsKey(item))
                    {
                        MapPresetsPref.CustomMapPresets[item] = new JObject()
                        {
                            {"paths", new JArray()
                                {
                                    new JObject()
                                    {
                                        {"name", "Default"},
                                        {"path", "{mapper}/{mod}"}
                                    }
                                }
                            }
                        };
                        ModTab.RefreshPresets();
                    }

                    UI.AddFormToWidget("Edelweiss:Forms/mapping_file_preset", "Modding/MappingFilePreset", (JObject)MapPresetsPref.CustomMapPresets[item]);
                }
            }
        }
    }
}