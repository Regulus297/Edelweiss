using System.Collections.Generic;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;
using Edelweiss.Utils;

namespace Edelweiss.Mapping.Tools
{
    [BaseRegistryObject()]
    public abstract class MappingTool : PluginRegistryObject
    {
        int selectedMaterial = 0;
        int selectedLayer = 0;
        int selectedMode = 0;

        public virtual string DisplayName => Name.Substring(0, Name.Length - 4).CamelCaseToText();

        public virtual bool ClickingTriggersDrag => false;
        internal void MouseDown(string room, int x, int y)
        {
            if (ClickingTriggersDrag)
                MouseDrag(room, x, y);
            MouseClick(room, x, y);
        }

        public virtual void MouseClick(string room, int x, int y)
        {

        }

        public virtual void MouseDrag(string room, int x, int y)
        {

        }

        public virtual void MouseRelease(string room, int x, int y)
        {

        }

        public virtual List<string> Materials => [];
        public virtual List<string> Layers => [];
        public virtual List<string> Modes => [];
    }
}