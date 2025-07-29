from network import PacketReceiver, PyNetworkManager
from plugins import plugin_loadable, JSONPreprocessor, load_dependencies, get_extra_data_safe
from ui import MappingWindow, JSONWidgetLoader
from Edelweiss.Network import Netcode
from PyQt5.QtWidgets import QLineEdit
import json


@load_dependencies("../common_code.py")
@plugin_loadable
class OpenPopupReceiver(PacketReceiver):
    def __init__(self):
        self.active_forms = {}
        super().__init__(Netcode.OPEN_POPUP_FORM)

    def process_packet(self, packet):
        data = JSONPreprocessor.loads(packet.data)
        widget = JSONWidgetLoader.init_widget(data)
        self.active_forms[widget.objectName()] = widget


        if CommonVars.found_submit_button is None:
            print(f"Form {widget.objectName()} does not have a submit button")
        else:
            CommonVars.found_submit_button.clicked.connect(lambda: widget.close())
            if "onsubmit" in data.keys():
                name = widget.objectName()

                if "extraData" not in data.keys():
                    data["extraData"] = {}

                for child in widget.children():
                    if isinstance(child, QLineEdit) and hasattr(child, "__json_data__") and child.objectName() != "":
                        child_data = getattr(child, "__json_data__")
                        lineEditType = "str" if "dataType" not in child_data.keys() else child_data["dataType"]

                        child_tracker = f"form_{name}/{child.objectName()}"
                        MappingWindow.instance.trackedWidgets[child_tracker] = child
                        setattr(child, "__tracked_as__", child_tracker)
                        
                        data["extraData"][child.objectName()] = {
                            "type": lineEditType,
                            "value": JSONPreprocessor.preprocess(f"@defer('@widget_property(\\'{child_tracker}\\', \\'text()\\')')")
                        }


                CommonVars.found_submit_button.clicked.connect(lambda: PyNetworkManager.send_packet(data["onsubmit"], json.dumps({
                    "id": name,
                    "extraData": get_extra_data_safe(data)
                })))    
        CommonVars.found_submit_button = None

        widget.setObjectName("popup")
        widget.setStyleSheet(MappingWindow.stylesheet)

        widget.show()
