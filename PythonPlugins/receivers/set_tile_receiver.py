from network import PacketReceiver
from plugins import plugin_loadable, JSONPreprocessor
from ui import MappingWindow, CustomDrawItem
from Edelweiss.Network import Netcode


@plugin_loadable
class SetTileReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.Get("Edelweiss:SetTile"))

    def process_packet(self, packet):
        data = JSONPreprocessor.loads(packet.data)
        
