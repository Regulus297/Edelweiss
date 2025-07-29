from network import PacketReceiver
from plugins import plugin_loadable
from ui import MappingWindow
from Edelweiss.Network import Netcode


@plugin_loadable
class AddShapeReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.ADD_SHAPE)

    def process_packet(self, packet):
        MappingWindow.instance.scene.addShape(packet.data)