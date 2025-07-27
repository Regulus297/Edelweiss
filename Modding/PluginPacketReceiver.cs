using System.Collections.Generic;
using Edelweiss.Network;
using Edelweiss.RegistryTypes;

namespace Edelweiss.Plugins
{
    [BaseRegistryObject()]
    public abstract class PluginPacketReceiver : PluginRegistryObject
    {
        
        public static readonly Dictionary<long, List<PluginPacketReceiver>> receivers = [];
        public abstract long HandledCode { get; }
        public abstract void ProcessPacket(Packet packet);

        public sealed override void Load()
        {
            if (!receivers.ContainsKey(HandledCode))
                receivers[HandledCode] = [];

            receivers[HandledCode].Add(this);
        }

        public virtual void PostLoad()
        {
            
        }
    }
}