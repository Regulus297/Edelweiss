using System.Collections.Generic;
using System.Linq;

namespace Edelweiss.Mapping.Tools
{
    public class RectTool : MappingTool
    {
        public override List<string> Materials => MainPlugin.Instance.fgTiles.Select(t => t.Value.name).ToList();
        public override List<string> Layers => ["Foreground", "Background"];
        public override List<string> Modes => ["Fill", "Line"];
    }
}