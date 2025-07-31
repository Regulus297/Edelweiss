from network import PacketReceiver, SyncedVariables
from plugins import plugin_loadable, load_dependencies, JSONPreprocessor
from ui import MappingWindow, JSONToolbarLoader
from Edelweiss.Network import Netcode


@plugin_loadable
class SyncVariableReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.SYNC_VARIABLE)

    def process_packet(self, packet):
        data = JSONPreprocessor.loads(packet.data)
        SyncedVariables.variables[data["name"]] = data["value"]
