from network.network_manager import PyNetworkManager



class PacketReceiver:
    def __init__(self, code):
        PyNetworkManager.receivers[code] = self

    def process_packet(self, packet):
        ...