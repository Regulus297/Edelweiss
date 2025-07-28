using Edelweiss.Plugins;

namespace Edelweiss.Tabs
{
    public class ResourcesTab : CustomTab
    {
        public override string LayoutJSON => "Edelweiss:resources_tab";

        public override string ToolbarJSON => "Edelweiss:resources_toolbar";

        public override string DisplayName => "Resources";
    }
}