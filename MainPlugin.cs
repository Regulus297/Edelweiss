using Edelweiss.Plugins;

namespace Edelweiss
{
    internal sealed class MainPlugin : Plugin
    {
        public override string ID => "Edelweiss";

        public long NetcodeDynamic { get; private set; }

        public override void Load()
        {
            NetcodeDynamic = CreateNetcode(nameof(NetcodeDynamic), false);
        }
    }
}