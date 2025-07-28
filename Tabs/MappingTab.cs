using Edelweiss.Plugins;

namespace Edelweiss.Tabs
{
    [LoadAfter(typeof(ResourcesTab))]
    public class MappingTab : CustomTab
    {
        public override string LayoutJSON => "Edelweiss:mapping_tab";

        public override string ToolbarJSON => "Edelweiss:mapping_toolbar";

        public override string DisplayName => "Mapping";
    }
}