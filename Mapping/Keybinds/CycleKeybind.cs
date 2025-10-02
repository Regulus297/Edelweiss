using System.Collections.Generic;
using Edelweiss.Plugins;

namespace Edelweiss.Mapping.Keybinds
{
    /// <summary>
    /// The keybind for cycling the state of an object. Defaults to E
    /// </summary>
    public class CycleKeybind : PluginKeybind
    {
        /// <inheritdoc/>
        public override List<int> DefaultBindings => [ReverseQtKeyNames["E"]];
    }
}