using System;
using System.Collections.Generic;
using System.Linq;

namespace Edelweiss.Mapping.Tools
{
    public class BrushTool : MappingTool
    {
        public override List<string> Materials => MainPlugin.Instance.fgTiles.Select(t => t.Value.name).ToList();
        public override List<string> Layers => ["Foreground", "Background"];
        public override bool ClickingTriggersDrag => true;

        public override void MouseDrag(string room, float x, float y)
        {
            Console.WriteLine("Brush!");
        }
    }
}