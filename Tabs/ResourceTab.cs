using Edelweiss.Plugins;

namespace Edelweiss.Tabs
{
    [LoadAfter(typeof(MappingTab))]
    public class ResourcesTab : CustomTab
    {
        public override string LayoutJSON => "Edelweiss:resources_tab";

        public override string ToolbarJSON => "Edelweiss:resources_toolbar";

        public override string DisplayName => "Resources";
    }
}