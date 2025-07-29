from network import PacketReceiver, PyNetworkManager
from plugins import plugin_loadable
from Edelweiss.Network import Netcode


@plugin_loadable
class QuitReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.QUIT)

    def process_packet(self, packet):
        PyNetworkManager.halt = True