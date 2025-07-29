from network import PacketReceiver
from plugins import plugin_loadable
from ui import MappingWindow
from Edelweiss.Network import Netcode


@plugin_loadable
class RegisterSceneReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.REGISTER_SCENE)

    def process_packet(self, packet):
        MappingWindow.instance.register_scene_json(packet.data)
