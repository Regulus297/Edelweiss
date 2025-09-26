using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Network
{
    /// <summary>
    /// A variable that is synced between backend and frontend automatically whenever its value is set
    /// </summary>
    public class SyncedVariable
    {
        private string name;
        private object value;
        private string[] syncedWidgets = [];

        /// <summary>
        /// Invoked when the variable's value is changed
        /// </summary>
        public event Action<object> OnChanged;

        /// <summary>
        /// The value of the variable. Converted to a JObject when synced to the frontend.
        /// </summary>
        public object Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                OnChanged?.Invoke(value);
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

        /// <summary>
        /// Instantiates a synced variable
        /// </summary>
        /// <param name="name">The name of the synced variable</param>
        /// <param name="defaultValue">The default value of the variable</param>
        /// <param name="syncedWidgets">The widgets that should refresh when this variable is set</param>
        public SyncedVariable(string name, object defaultValue = null, params string[] syncedWidgets)
        {
            this.name = name;
            value = defaultValue;
            this.syncedWidgets = syncedWidgets;
            NetworkManager.SendPacket(Netcode.SYNC_VARIABLE, new JObject()
            {
                {"name", name},
                {"value", JToken.FromObject(defaultValue ?? "")}
            });
        }
    }
}