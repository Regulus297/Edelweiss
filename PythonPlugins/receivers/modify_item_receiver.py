from network import PacketReceiver
from plugins import plugin_loadable, load_dependencies, JSONPreprocessor
from ui import MappingWindow, CustomDrawItem
from ui.opengl import GLWidget
from Edelweiss.Network import Netcode


@load_dependencies("../widgets/zoomable_view.py")
@plugin_loadable
class ModifyItemShapeReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.MODIFY_ITEM)

    def process_packet(self, packet):
        data = JSONPreprocessor.loads(packet.data)
        widget = MappingWindow.instance.get_tracked_widget(data["widget"])
        if type(widget) == ZoomableView:
            self._handle_zoomable(widget, data)
        elif type(widget) == GLWidget:
            self._handle_gl(widget, data)
        
    def _handle_gl(self, widget, data):
        if data["item"] not in widget.items:
            print(f"No item found with name {data['item']}")
            return
        
        item = widget.items[data["item"]]
        if "action" not in data or data["action"] == "modify":
            item.refresh(data["data"])
        elif data["action"] == "add":
            for shape in data["shapes"]:
                item.addShape(shape)
        elif data["action"] == "clear":
            item.clear()
        elif data["action"] == "delete":
            item.delete()
        
    def _handle_zoomable(self, widget, data):
        if data["item"] not in widget.trackedItems.keys():
            print(f"No item found with name {data['item']}")
            return
        
        item = widget.trackedItems[data["item"]]
        gItem = widget.trackedGraphicsItems[data["item"]]
        if "action" not in data or data["action"] == "modify":
            widget.trackedItems[data["item"]].update(data["data"])
            widget.trackedGraphicsItems[data["item"]].refresh(data["data"], "shapes" in data["data"])
        elif data["action"] == "add":
            for shape in data["shapes"]:
                gItem.addShape(shape)
            gItem.refresh({})
        elif data["action"] == "clear":
            gItem.clear()
        elif data["action"] == "delete":
            gItem.delete()
            del widget.trackedItems[data["item"]]
            del widget.trackedGraphicsItems[data["item"]]
