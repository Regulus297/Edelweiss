using Newtonsoft.Json.Linq;

namespace Edelweiss.Network
{
    public class SyncedVariable(string name)
    {
        private string name = name;
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
    }
}