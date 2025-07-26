from network.packet_receiver import PacketReceiver
from network.network_manager import PyNetworkManager
from plugins.plugin_loadable import plugin_loadable
from Edelweiss.Network import Netcode


@plugin_loadable
class QuitReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.QUIT)

    def process_packet(self, packet):
        print("HALT")
        PyNetworkManager.halt = True