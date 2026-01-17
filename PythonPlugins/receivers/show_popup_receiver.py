from network import PacketReceiver
from plugins import plugin_loadable, load_dependencies, JSONPreprocessor
from ui import MappingWindow
from Edelweiss.Network import Netcode

@load_dependencies("../widgets/popup.py")
@plugin_loadable
class ShowPopupReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.SHOW_POPUP)

    def process_packet(self, packet):
        data = JSONPreprocessor.loads(packet.data)
        duration = data["duration"] if "duration" in data else 2000
        Popup(data["text"], None, duration).show()