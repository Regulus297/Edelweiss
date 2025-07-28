using Newtonsoft.Json.Linq;

namespace Edelweiss.Network
{
    /// <summary>
    /// The class that carries data between backend and UI and vice versa
    /// </summary>
    /// <param name="code">The <see cref="Netcode"/> of the packet</param>
    /// <param name="data">The data the packet contains</param>
    public class Packet(long code, string data)
    {
        /// <summary>
        /// The <see cref="Netcode"/> of the packet
        /// </summary>
        public long code = code;

        /// <summary>
        /// The data the packet contains
        /// </summary>
        public string data = data;
    }
}