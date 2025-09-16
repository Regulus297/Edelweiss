using System.Collections.Generic;
using Edelweiss.Plugins;

namespace Edelweiss.Mapping.Keybinds
{
    /// <summary>
    /// Keybind for flipping an object horizontally. Defaults to H
    /// </summary>
    public class HorizontalFlipKeybind : PluginKeybind
    {
        /// <inheritdoc/>
        public override List<int> DefaultBindings => [ReverseQtKeyNames["H"]];
    }
}