using Edelweiss.Modding;
using Edelweiss.Plugins;

namespace Edelweiss.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    [LoadAfter(typeof(ModdingTab))]
    public class MappingTab : CustomTab
    {
        /// <inheritdoc/>
        public override string LayoutJSON => "Edelweiss:mapping_tab";

        /// <inheritdoc/>
        public override string ToolbarJSON => "Edelweiss:mapping_toolbar";

        /// <inheritdoc/>
        public override string DisplayName => "Mapping";
    }
}