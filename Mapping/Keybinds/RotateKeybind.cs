using System.Collections.Generic;
using Edelweiss.Plugins;

namespace Edelweiss.Mapping.Keybinds
{
    /// <summary>
    /// The keybind for rotating an object. Bound to R by default
    /// </summary>
    public class RotateKeybind : PluginKeybind
    {
        /// <inheritdoc/>
        public override List<int> DefaultBindings => [ReverseQtKeyNames["R"]];
    }
}