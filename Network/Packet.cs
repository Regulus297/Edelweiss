using Newtonsoft.Json.Linq;

namespace Edelweiss.Network
{
    public class Packet(long code, string data)
    {
        public long code = code;
        public string data = data;
    }
}