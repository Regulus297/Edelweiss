using Edelweiss.MVC;
using Edelweiss.Plugins;

namespace Edelweiss.Modding
{
    internal class MappingTab : CustomTab
    {
        public override string LayoutJSON => "";

        public override string ToolbarJSON => "Edelweiss:mapping_toolbar";

        public override string DisplayName => "Mapping";
    }
}