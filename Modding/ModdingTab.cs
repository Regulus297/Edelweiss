using Edelweiss.MVC;
using Edelweiss.Plugins;

namespace Edelweiss.Modding
{
    internal class ModdingTab : CustomTab
    {
        public override string LayoutJSON => "";

        public override string ToolbarJSON => "Edelweiss:mod_toolbar";

        public override string DisplayName => "Modding";
    }
}