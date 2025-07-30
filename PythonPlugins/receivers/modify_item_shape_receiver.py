from network import PacketReceiver
from plugins import plugin_loadable, load_dependencies, JSONPreprocessor
from ui import MappingWindow, CustomDrawItem
from Edelweiss.Network import Netcode


@load_dependencies("../widgets/zoomable_view.py")
@plugin_loadable
class ModifyItemShapeReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.MODIFY_ITEM_SHAPE)

    def process_packet(self, packet):
        data = JSONPreprocessor.loads(packet.data)
        widget = MappingWindow.instance.get_tracked_widget(data["widget"])
        if type(widget) != ZoomableView:
            print(f"Failed to add item as widget {type(widget)} is not a {ZoomableView}")
            return
        
        if data["item"] not in widget.trackedItems.keys():
            print(f"No item found with name {data['item']}")
            return
        


        shape = widget.trackedItems[data["item"]]["shapes"][data["index"]]
        for key, value in data["data"].items():
            shape[key] = value
