using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Network
{
    public class SyncedVariable
    {
        private string name;
        private object value;
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
            }
        }

        public SyncedVariable(string name, object defaultValue = null)
        {
            this.name = name;
            NetworkManager.SendPacket(Netcode.SYNC_VARIABLE, new JObject()
            {
                {"name", name},
                {"value", JToken.FromObject(defaultValue ?? "")}
            });
        }
    }
}