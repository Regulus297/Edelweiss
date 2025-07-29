from network import PacketReceiver
from plugins import plugin_loadable, load_dependencies, JSONPreprocessor
from ui import MappingWindow, JSONToolbarLoader
from Edelweiss.Network import Netcode


@load_dependencies("../common_code.py")
@plugin_loadable
class RegisterToolbarReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.REGISTER_TOOLBAR)

    def process_packet(self, packet):
        JSONToolbarLoader.init_toolbar(MappingWindow.instance.tool_bar, JSONPreprocessor.loads(packet.data), clear_main_toolbar)
