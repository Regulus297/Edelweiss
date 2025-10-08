from network import PacketReceiver, PyNetworkManager
from plugins import plugin_loadable, JSONPreprocessor, load_dependencies, get_extra_data_safe
from ui import MappingWindow, JSONWidgetLoader
from Edelweiss.Network import Netcode
from PyQt5.QtWidgets import QLineEdit, QComboBox, QCheckBox
import json


@load_dependencies("../common_code.py")
@plugin_loadable
class OpenPopupReceiver(PacketReceiver):
    def __init__(self):
        self.active_forms = {}
        self.handled_widget_types = [QLineEdit, QComboBox, QCheckBox]
        super().__init__(Netcode.OPEN_POPUP_FORM)

    def process_packet(self, packet):
        data = JSONPreprocessor.loads(packet.data)
        widget = JSONWidgetLoader.init_widget(data)
        self.active_forms[widget.objectName()] = widget

        validators = {}


        if CommonVars.found_submit_button is None:
            print(f"Form {widget.objectName()} does not have a submit button")
        else:
            
            def submit(data, name):
                for validator, w in validators.items():
                    if not validator.valid(w):
                        return

                extraData = get_extra_data_safe(data)
                for child in widget.children():
                    if not(hasattr(child, "__json_data__") and child.objectName() != ""):
                        continue
                    child_data = getattr(child, "__json_data__")
                    child_tracker = f"form_{name}/{child.objectName()}"
                    if isinstance(child, QLineEdit):
                        lineEditType = "str" if "dataType" not in child_data.keys() else child_data["dataType"]
                        extraData[child.objectName()] = get_extra_data_safe({
                            "extraData": {
                                "text": {
                                    "type": lineEditType,
                                    "value": child.text()
                                }
                            }
                        })["text"]
                    elif isinstance(child, QComboBox):
                        extraData[child.objectName()] = OpenPopupReceiver.get_combobox_data(child)
                    elif isinstance(child, QCheckBox):
                        extraData[child.objectName()] = child.isChecked()
                PyNetworkManager.send_packet(data["onsubmit"], json.dumps({
                    "id": name,
                    "extraData": extraData
                }))
                widget.close()
                

            if "onsubmit" in data.keys():
                name = widget.objectName()

                if "extraData" not in data.keys():
                    data["extraData"] = {}

                for child in widget.children():
                    if not(hasattr(child, "__json_data__") and child.objectName() != ""):
                        continue

                    if type(child) in self.handled_widget_types:
                        child_data = getattr(child, "__json_data__")
                        child_tracker = f"form_{name}/{child.objectName()}"
                        MappingWindow.instance.trackedWidgets[child_tracker] = child
                        setattr(child, "__tracked_as__", child_tracker)

                    if isinstance(child, QLineEdit):
                        lineEditType = "str" if "dataType" not in child_data.keys() else child_data["dataType"]     
                        if hasattr(child, "__custom_validator__"):
                            validators[getattr(child, "__custom_validator__")] = child




                CommonVars.found_submit_button.clicked.connect(lambda: submit(data, name))    
        CommonVars.found_submit_button = None

        widget.setObjectName("popup")
        widget.setStyleSheet(MappingWindow.stylesheet)

        widget.show()

    @staticmethod
    def get_combobox_data(combobox):
        text = combobox.currentText()
        if combobox.isEditable():
            custom = True
            for i in range(combobox.count()):
                if text == combobox.itemText(i):
                    custom = False
                    break
            if custom:
                return text
        if combobox.currentData() is not None:
            return combobox.currentData()
        return text