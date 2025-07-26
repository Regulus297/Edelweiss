from network.packet_receiver import PacketReceiver
from network.network_manager import PyNetworkManager
from plugins.plugin_loadable import plugin_loadable
from ui.main_window import MappingWindow
from Edelweiss.Network import Netcode


@plugin_loadable
class AddItemReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.ADD_ITEM)

    def process_packet(self, packet):
        print("E")
        MappingWindow.instance.scene.addItem(packet.data)