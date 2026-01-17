using System;
using System.Threading.Tasks;
using Edelweiss.Mapping.SaveLoad;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.PacketReceivers
{   
    [LoadAfter(typeof(MappingTab))]
    internal class MapFileReceiver : PacketReceiver
    {
        public override long HandledCode => MappingTab.MapFileNetcode;

        public override void ProcessPacket(Packet packet)
        {
            JObject data = JObject.Parse(packet.data);
            string operation = data.Value<JObject>("extraData").Value<string>("type");
            Task.Run(() =>
            {
                try
                {
                    MappingTab.filePath = data["path"].ToString();
                    if (operation == "save") {
                        MapSaveLoad.SaveMap(MappingTab.map, MappingTab.filePath);
                        UI.ShowLocalizedPopup("Edelweiss.Mapping.SavedMap");
                    }
                    else if (operation == "load")
                    {
                        MappingTab.map = MapSaveLoad.LoadMap(MappingTab.filePath);
                        NetworkManager.SendPacket(Netcode.CLEAR_VIEW, new JObject()
                        {
                            {"widget", "Mapping/MainView"}
                        });

                        // Re-add the cursor ghost
                        NetworkManager.SendPacket(Netcode.ADD_ITEM, new JObject()
                        {
                            {"widget", "Mapping/MainView"},
                            {"item", PluginLoader.RequestJObject("Edelweiss:GraphicsItems/cursor_ghost")}
                        });

                        MappingTab.RedrawComplete();
                    }
                }
                catch (Exception e)
                {
                    MainPlugin.Instance.Logger.Error($"Error saving map: {e}");
                }
            });
        }
    }
}