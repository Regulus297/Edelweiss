using System;
using System.Collections.Generic;
using System.Drawing;
using Edelweiss.Mapping.Entities;
using Edelweiss.Mapping.Keybinds;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Tools
{
    [LoadAfter(typeof(EntityTool))]
    internal class SelectionTool : MappingTool
    {
        internal static SyncedVariable Active = new SyncedVariable("Edelweiss:SelectionActive", false);
        internal static long SelectionMovedNetcode { get; private set; }
        internal static long SelectionChangedNetcode { get; private set; }
        internal static long SelectionResizedNetcode { get; private set; }
        internal static long RightClickedNetcode { get; private set; }

        internal static List<(Entity, int)> selected = [];

        internal bool dragging = false;
        internal int startX, startY;

        public override void Load()
        {
            Active.Value = false;
            SelectionMovedNetcode = Plugin.CreateNetcode("SelectionMoved", false);
            SelectionChangedNetcode = Plugin.CreateNetcode("SelectionChanged", false);
            SelectionResizedNetcode = Plugin.CreateNetcode("SelectionResized", false);
            RightClickedNetcode = Plugin.CreateNetcode("RightClickedNetcode", false);

            PluginKeybind.AddListener<CycleKeybind>(() => DoForAllSelected((e, _) => e.Cycle(1)));
            PluginKeybind.AddListener<RotateKeybind>(() => DoForAllSelected((e, _) => e.Rotate(1)));
            PluginKeybind.AddListener<HorizontalFlipKeybind>(() => DoForAllSelected((e, _) => e.Flip(true, false)));
            PluginKeybind.AddListener<VerticalFlipKeybind>(() => DoForAllSelected((e, _) => e.Flip(false, true)));
            PluginKeybind.AddListener<NodeKeybind>(() => DoForAllSelected((e, _) =>
            {
                e.AddNode(new Point(e.nodes.Count * 32 + 32, 0));
            }));
            PluginKeybind.AddListener<DeleteKeybind>(() =>
            {
                Dictionary<Entity, List<int>> nodesToRemove = [];
                DoForAllSelected((e, node) =>
                {
                    if (!nodesToRemove.TryGetValue(e, out var nodes))
                    {
                        nodesToRemove[e] = [node - 1];
                    }
                    else
                    {
                        nodes.Add(node - 1);
                    }
                }, false);

                RoomData room = null;
                foreach (var pair in nodesToRemove)
                {
                    Entity e = pair.Key;
                    room = e.entityRoom;
                    pair.Value.Sort((int a, int b) => b - a);
                    foreach (int node in pair.Value)
                    {
                        if (node < 0 || !e.TryRemoveNode(node))
                        {
                            e.entityRoom?.RemoveEntity(e);
                            NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
                            {
                                {"widget", "Mapping/MainView"},
                                {"item", $"{e.entityRoom.name}/{e.entityRoom.name}:{e._id}"},
                                {"action", "delete"}
                            });
                            if (!Entity.DiscardedIDs.ContainsKey(room))
                                Entity.DiscardedIDs[room] = [];
                            Entity.DiscardedIDs[room].Enqueue(e._id);
                        }
                    }
                }
                room?.RedrawEntities();

                selected.Clear();
            });
        }

        internal void DoForAllSelected(Action<Entity, int> action, bool redraw = true)
        {
            if (MappingTab.selectedTool != this)
                return;

            RoomData room = null;
            foreach ((Entity entity, int node) in selected)
            {
                action(entity, node);
                room = entity.entityRoom;
            }
            if(redraw)
                room?.RedrawEntities();
        }

        public override void OnSelect()
        {
            Active.Value = true;
            NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", "cursorGhost"},
                {"data", new JObject() {
                    {"shapes", new JArray() {
                        new JObject() {
                            {"type", "rectangle"},
                            {"color", "#6d9eed"},
                            {"fill", "#446d9eed"},
                            {"thickness", 0.5f},
                            {"x", 0},
                            {"y", 0},
                            {"width", 0},
                            {"height", 0},
                            {"depth", int.MinValue}
                        }
                    }}
                }}
            });
        }

        public override void MouseClick(JObject room, float x, float y)
        {
            dragging = true;
            startX = (int)x;
            startY = (int)y;
        }

        public override void MouseDrag(JObject room, float x, float y)
        {
            NetworkManager.SendPacket(Netcode.MODIFY_ITEM_SHAPE, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", "cursorGhost"},
                {"index", 0},
                { "data", new JObject() {
                    {"width", (int)x-startX},
                    {"height", (int)y - startY}
                }}
            });
        }

        public override void MouseRelease(JObject room, float x, float y)
        {
            dragging = false;
            NetworkManager.SendPacket(Netcode.MODIFY_ITEM_SHAPE, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", "cursorGhost"},
                {"index", 0},
                { "data", new JObject() {
                    {"width", 0},
                    {"height", 0}
                }}
            });
        }

        public override bool UpdateCursorGhost(float mouseX, float mouseY)
        {
            return dragging;
        }

        public override void OnDeselect()
        {
            Active.Value = false;
            NetworkManager.SendPacket(Netcode.MODIFY_ITEM, new JObject()
            {
                {"widget", "Mapping/MainView"},
                {"item", "cursorGhost"},
                {"data", new JObject() {
                    {"shapes", new JArray() {
                        new JObject() {
                            {"type", "tileGhost"},
                            {"color", "#aaaaaa"},
                            {"thickness", "@defer('@pen_thickness(\\'Mapping/MainView\\')')"},
                            {"width", 8},
                            {"height", 8},
                            {"depth", int.MinValue},
                            { "coords", new JArray() {
                                "0,0"
                            }},
                            { "visible", true}
                        }
                    }}
                }}
            });
        }
    }
}