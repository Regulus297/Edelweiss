from network import PacketReceiver
from plugins import plugin_loadable, load_dependencies, JSONPreprocessor
from ui import MappingWindow, CustomDrawItem, LoadingScreen
from Edelweiss.Network import Netcode


@plugin_loadable
class LoadingScreenReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.MODIFY_LOADING_SCREEN)

    def process_packet(self, packet):
        data = JSONPreprocessor.loads(packet.data)
        action = data["action"]
        if action == "open":
            MappingWindow.instance.loadingScreen = LoadingScreen.create_from_name(data["name"], MappingWindow.instance)
            MappingWindow.instance.loadingScreen.show()
        elif action == "modify":
            MappingWindow.instance.loadingScreen.progress = data["progress"]
            MappingWindow.instance.loadingScreen.update()
        elif action == "close":
            MappingWindow.instance.loadingScreen.close()
            MappingWindow.instance.loadingScreen = None
