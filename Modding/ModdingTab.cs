using Edelweiss.Plugins;

namespace Edelweiss.Modding
{
    public class ModdingTab : CustomTab
    {
        public override string LayoutJSON => "Edelweiss:mod_tab";

        public override string ToolbarJSON => "";

        public override string DisplayName => "Modding";
    }
}