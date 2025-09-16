using System.Collections.Generic;
using Edelweiss.Plugins;

namespace Edelweiss.Mapping.Keybinds
{
    /// <summary>
    /// Keybind for flipping an object vertically. Defaults to V
    /// </summary>
    public class VerticalFlipKeybind : PluginKeybind
    {
        /// <inheritdoc/>
        public override List<int> DefaultBindings => [ReverseQtKeyNames["V"]];
    }
}