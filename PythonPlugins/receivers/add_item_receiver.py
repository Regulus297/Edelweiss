from network import PacketReceiver
from plugins import plugin_loadable, load_dependencies, JSONPreprocessor
from ui import MappingWindow, CustomDrawItem
from Edelweiss.Network import Netcode

from ui.opengl import GLWidget, GLItem


@load_dependencies("../widgets/zoomable_view.py")
@plugin_loadable
class AddItemReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.ADD_ITEM)

    def process_packet(self, packet):
        data = JSONPreprocessor.loads(packet.data)
        widget = MappingWindow.instance.get_tracked_widget(data["widget"])
        if type(widget) == ZoomableView:
            self._add_item_zoomable(widget, data)
            return
        elif type(widget) == GLWidget:
            self._add_item_gl(widget, data)
            return
        
    def _add_item_gl(self, widget, data):
        parent = None
        if "parent" in data and data["parent"] in widget.items:
            parent = widget.items[data["parent"]]

        item = GLItem(data["item"], parent)
        widget.addItem(item)


    def _add_item_zoomable(self, widget, data):
        parent = None
        if "parent" in data and data["parent"] in widget.trackedItems:
            parent = widget.trackedGraphicsItems[data["parent"]]

        item = CustomDrawItem(data["item"], parent)

        identifier = ((getattr(parent, "identifier", "") + "/") if parent is not None else "") + data["item"]["name"]
        item.identifier = identifier

        widget.trackedItems[identifier] = data["item"]
        widget.trackedGraphicsItems[identifier] = item
        widget.grScene.addItem(item)