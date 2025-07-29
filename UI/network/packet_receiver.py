from network import PyNetworkManager



class PacketReceiver:
    def __init__(self, code):
        if code not in PyNetworkManager.receivers.keys():
            PyNetworkManager.receivers[code] = []
        PyNetworkManager.receivers[code].append(self)

    def process_packet(self, packet):
        ...