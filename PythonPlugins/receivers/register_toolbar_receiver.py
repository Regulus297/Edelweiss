from network.packet_receiver import PacketReceiver
from plugins.plugin_loadable import plugin_loadable
from plugins.load_dependencies import load_dependencies
from ui.main_window import MappingWindow
from Edelweiss.Network import Netcode
from ui.json_toolbar_loader import JSONToolbarLoader
import json


@load_dependencies("../common_code.py")
@plugin_loadable
class RegisterToolbarReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.REGISTER_TOOLBAR)

    def process_packet(self, packet):
        JSONToolbarLoader.init_toolbar(MappingWindow.instance.tool_bar, json.loads(packet.data), clear_main_toolbar)
