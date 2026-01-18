using Edelweiss.Plugins;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.ModManagement
{
    internal class ModTab : CustomTab
    {
        public override string LayoutJSON => "Edelweiss:mod_tab";
        public override string ToolbarJSON => "Edelweiss:mod_toolbar";
        public override string DisplayName => "Mod";

        public static long CreateModNetcode { get; private set; }

        public override void Load()
        {
            CreateModNetcode = Plugin.CreateNetcode("CreateMod", false);
        }

        public override void HandleToolbarClick(string actionName, JObject extraData)
        {
            switch(actionName) {
                case "modMenu/createMod":
                    UI.OpenForm("Edelweiss:Forms/mod_creation");
                    break;
            }
        }

        private void CreateMod(string name)
        {
            
        }
    }
}