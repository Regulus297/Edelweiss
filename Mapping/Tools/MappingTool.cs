using System.Collections.Generic;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Tools
{
    [BaseRegistryObject()]
    public abstract class MappingTool : PluginRegistryObject
    {
        internal string selectedMaterial = "";
        internal int selectedLayer = 0;
        internal int selectedMode = 0;

        public virtual string DisplayName => Name.Substring(0, Name.Length - 4).CamelCaseToText();

        public virtual bool ClickingTriggersDrag => false;
        internal void MouseDown(JObject room, float x, float y)
        {
            if (ClickingTriggersDrag)
                MouseDrag(room, x, y);
            MouseClick(room, x, y);
        }

        public virtual void MouseClick(JObject room, float x, float y)
        {

        }

        public virtual void MouseDrag(JObject room, float x, float y)
        {

        }

        public virtual void MouseRelease(JObject room, float x, float y)
        {

        }

        public virtual List<string> Materials => [];
        public virtual List<string> MaterialIDs => [];
        public virtual List<string> Layers => [];
        public virtual List<string> Modes => [];
    }
}