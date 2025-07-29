from network import PacketReceiver
from plugins import plugin_loadable, JSONPreprocessor
from ui import MappingWindow, JSONWidgetLoader
from Edelweiss.Network import Netcode
import json


@plugin_loadable
class OpenPopupReceiver(PacketReceiver):
    def __init__(self):
        self.active_forms = {}
        super().__init__(Netcode.OPEN_POPUP_FORM)

    def process_packet(self, packet):
        widget = JSONWidgetLoader.init_widget(JSONPreprocessor.loads(packet.data))
        self.active_forms[widget.objectName()] = widget
        widget.setObjectName("popup")
        widget.setStyleSheet(MappingWindow.stylesheet)
        widget.show()
