from network.packet_receiver import PacketReceiver
from network.network_manager import PyNetworkManager
from plugins.plugin_loadable import plugin_loadable
from ui.main_window import MappingWindow
from ui.json_widget_loader import JSONWidgetLoader
from Edelweiss.Network import Netcode
import json


@plugin_loadable
class RegisterJsonSceneReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.REGISTER_JSON_SCENE)

    def process_packet(self, packet):
        data = json.loads(packet.data)
        MappingWindow.instance.register_scene(JSONWidgetLoader.init_widget(data, MappingWindow.instance.stack), data["name"])
