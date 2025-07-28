using Edelweiss.Network;
using Edelweiss.RegistryTypes;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Plugins
{
    [BaseRegistryObject()]
    public abstract class CustomTab : PluginRegistryObject
    {
        public abstract string LayoutJSON { get; }
        public abstract string ToolbarJSON { get; }
        public abstract string DisplayName { get; }
        public virtual int LoadWeight => 0;
        

        public override void PostSetupContent()
        {
            NetworkManager.SendPacket(Netcode.REGISTER_JSON_SCENE, new JObject()
            {
                {"name", DisplayName},
                {"internalName", FullName},
                {"json", PluginLoader.RequestJson(LayoutJSON)}
            });
        }
    }
}