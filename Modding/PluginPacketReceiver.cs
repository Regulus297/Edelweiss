using System;
using System.Collections.Generic;
using Edelweiss.Network;
using Edelweiss.RegistryTypes;

namespace Edelweiss.Plugins
{
    /// <summary>
    /// The base class for a packet receiver
    /// </summary>
    [BaseRegistryObject()]
    public abstract class PacketReceiver : PluginRegistryObject
    {
        /// <summary>
        /// All packet receivers, organized by the packet code that they handle.
        /// </summary>
        public static readonly Dictionary<long, List<PacketReceiver>> receivers = [];

        /// <summary>
        /// The specific <see cref="Netcode"/> that this receiver handles. 
        /// Whenever the backend receives a packet with this code, this class's <see cref="ProcessPacket(Packet)"/> method will be called on that packet.
        /// </summary>
        public abstract long HandledCode { get; }

        /// <summary>
        /// Called whenever the backend receives a packet with this receiver's <see cref="HandledCode"/>
        /// </summary>
        /// <param name="packet">The received packet</param>
        public abstract void ProcessPacket(Packet packet);

        /// <inheritdoc/>
        public sealed override void Load()
        {
            if (!receivers.ContainsKey(HandledCode))
                receivers[HandledCode] = [];

            receivers[HandledCode].Add(this);
            PostLoad();
        }

        /// <summary>
        /// Called after this class is loaded.
        /// </summary>
        public virtual void PostLoad()
        {
            
        }
    }
}