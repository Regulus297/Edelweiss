from network import PacketReceiver
from plugins import plugin_loadable, JSONPreprocessor, load_dependencies
from ui import MappingWindow, JSONWidgetLoader
from Edelweiss.Network import Netcode
import json


@load_dependencies("../common_code.py")
@plugin_loadable
class OpenPopupReceiver(PacketReceiver):
    def __init__(self):
        self.active_forms = {}
        super().__init__(Netcode.OPEN_POPUP_FORM)

    def process_packet(self, packet):
        widget = JSONWidgetLoader.init_widget(JSONPreprocessor.loads(packet.data))
        self.active_forms[widget.objectName()] = widget

        if CommonVars.found_submit_button is None:
            print(f"Form {widget.objectName()} does not have a submit button")
        else:
            CommonVars.found_submit_button.clicked.connect(lambda: widget.close())
        CommonVars.found_submit_button = None

        widget.setObjectName("popup")
        widget.setStyleSheet(MappingWindow.stylesheet)

        widget.show()
