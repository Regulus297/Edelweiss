using System.Collections.Generic;
using Edelweiss.Plugins;

namespace Edelweiss.Mapping.Keybinds
{
    /// <summary>
    /// The keybind for deleting an object. Defaults to Del
    /// </summary>
    public class DeleteKeybind : PluginKeybind
    {
        /// <inheritdoc/>
        public override List<int> DefaultBindings => [ReverseQtKeyNames["Delete"]];
    }
}