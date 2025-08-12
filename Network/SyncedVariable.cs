using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Network
{
    public class SyncedVariable
    {
        private string name;
        private object value;
        private string[] syncedWidgets = [];
        public object Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                NetworkManager.SendPacket(Netcode.SYNC_VARIABLE, new JObject()
                {
                    {"name", name},
                    {"value", JToken.FromObject(value)}
                });
                if (syncedWidgets.Length == 0)
                    return;

                NetworkManager.SendPacket(Netcode.REFRESH_WIDGETS, new JObject()
                {
                    {"widgets", new JArray(syncedWidgets)}
                });
            }
        }

        public SyncedVariable(string name, object defaultValue = null, params string[] syncedWidgets)
        {
            this.name = name;
            this.syncedWidgets = syncedWidgets;
            NetworkManager.SendPacket(Netcode.SYNC_VARIABLE, new JObject()
            {
                {"name", name},
                {"value", JToken.FromObject(defaultValue ?? "")}
            });
        }
    }
}