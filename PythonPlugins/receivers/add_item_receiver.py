from network.packet_receiver import PacketReceiver
from network.network_manager import PyNetworkManager
from plugins.plugin_loadable import plugin_loadable
from plugins.load_dependencies import load_dependencies
from plugins.json_preprocessor import JSONPreprocessor
from ui.main_window import MappingWindow
from Edelweiss.Network import Netcode
from ui.custom_draw_item import CustomDrawItem


@load_dependencies("../widgets/zoomable_view.py")
@plugin_loadable
class AddItemReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.ADD_ITEM)

    def process_packet(self, packet):
        data = JSONPreprocessor.loads(packet.data)
        widget = MappingWindow.instance.get_tracked_widget(data["widget"])
        # if type(widget) != ZoomableView:
        #     print(f"Failed to add item as widget {type(widget)} is not a {ZoomableView}")
        #     return

        widget.grScene.addItem(CustomDrawItem(data["item"]))