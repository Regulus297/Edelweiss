from network import PacketReceiver
from plugins import plugin_loadable, JSONPreprocessor
from ui import MappingWindow, JSONWidgetLoader
from Edelweiss.Network import Netcode

@plugin_loadable
class RegisterJsonSceneReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.REGISTER_TAB)

    def process_packet(self, packet):
        data = JSONPreprocessor.loads(packet.data)
        MappingWindow.instance.register_scene(JSONWidgetLoader.init_widget(JSONPreprocessor.loads(data["json"]), MappingWindow.instance.stack), data["name"], data["internalName"])
