using System;
using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Entities;
using Edelweiss.Mapping.Keybinds;
using Edelweiss.Network;
using Edelweiss.Plugins;

namespace Edelweiss.Mapping.Tools
{
    [LoadAfter(typeof(EntityTool))]
    internal class SelectionTool : MappingTool
    {
        internal static SyncedVariable Active = new SyncedVariable("Edelweiss:SelectionActive", false);
        internal static long SelectionMovedNetcode { get; private set; }
        internal static long SelectionChangedNetcode { get; private set; }
        internal static long SelectionResizedNetcode { get; private set; }

        internal static List<(Entity, int)> selected = [];

        public override void Load()
        {
            Active.Value = false;
            SelectionMovedNetcode = Plugin.CreateNetcode("SelectionMoved", false);
            SelectionChangedNetcode = Plugin.CreateNetcode("SelectionChanged", false);
            SelectionResizedNetcode = Plugin.CreateNetcode("SelectionResized", false);

            PluginKeybind.AddListener<CycleKeybind>(() => DoForAllSelected(e => e.Cycle(1)));
            PluginKeybind.AddListener<RotateKeybind>(() => DoForAllSelected(e => e.Rotate(1)));
            PluginKeybind.AddListener<HorizontalFlipKeybind>(() => DoForAllSelected(e => e.Flip(true, false)));
            PluginKeybind.AddListener<VerticalFlipKeybind>(() => DoForAllSelected(e => e.Flip(false, true)));
            PluginKeybind.AddListener<NodeKeybind>(() => DoForAllSelected(e =>
            {
                e.AddNode(new Point(e.nodes.Count * 32 + 32, 0));
            }));
        }

        private void DoForAllSelected(Action<Entity> action)
        {
            if (MappingTab.selectedTool != this)
                return;

            RoomData room = null;
            foreach ((Entity entity, int node) in selected)
            {
                action(entity);
                room = entity.entityRoom;
            }
            room?.RedrawEntities();
        }

        public override void OnSelect()
        {
            Active.Value = true;
        }

        public override void OnDeselect()
        {
            Active.Value = false;
        }
    }
}