using Newtonsoft.Json.Linq;

namespace Edelweiss.Network
{
    public class Packet(ulong code, string data)
    {
        public ulong code = code;
        public string data = data;
    }
}