from network import PacketReceiver
from plugins import plugin_loadable, load_dependencies, JSONPreprocessor
from ui import MappingWindow, CustomDrawItem, SelectionRect
from Edelweiss.Network import Netcode


@load_dependencies("../widgets/zoomable_view.py")
@plugin_loadable
class ClearViewReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.CLEAR_VIEW)

    def process_packet(self, packet):
        data = JSONPreprocessor.loads(packet.data)
        widget = MappingWindow.instance.get_tracked_widget(data["widget"])
        if type(widget) != ZoomableView:
            print(f"Failed to add item as widget {type(widget)} is not a {ZoomableView}")
            return
        
        SelectionRect.selection_rects.clear()
        widget.grScene.clear()