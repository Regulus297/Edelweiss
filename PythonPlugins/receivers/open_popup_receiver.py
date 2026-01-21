from network import PacketReceiver, PyNetworkManager
from plugins import plugin_loadable, JSONPreprocessor, load_dependencies, get_extra_data_safe
from ui import MappingWindow, JSONWidgetLoader, FormList
from Edelweiss.Network import Netcode
from PyQt5.QtWidgets import QLineEdit, QComboBox, QCheckBox, QPushButton
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

        old = self.active_forms.get(widget.objectName())
        if data.get("replace"):
            if old is not None:
                old.close()
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
                self.get_data(widget, extraData)
                PyNetworkManager.send_packet(data["onsubmit"], json.dumps({
                    "id": name,
                    "extraData": extraData
                }))

                if "parent" not in data:
                    widget.close()
                

            if "onsubmit" in data.keys():
                name = widget.objectName()

                if "extraData" not in data.keys():
                    data["extraData"] = {}

                for child in widget.children():
                    if not(hasattr(child, "__json_data__") and child.objectName() != ""):
                        continue

                    if isinstance(child, QLineEdit):
                        child_data = getattr(child, "__json_data__")
                        lineEditType = "str" if "dataType" not in child_data.keys() else child_data["dataType"]     
                        if hasattr(child, "__custom_validator__"):
                            validators[getattr(child, "__custom_validator__")] = child

                CommonVars.found_submit_button.clicked.connect(lambda: submit(data, name))    
        CommonVars.found_submit_button = None

        if "parent" not in data:
            widget.setObjectName("popup")
            widget.setStyleSheet(MappingWindow.stylesheet)

            widget.show()
        else:
            parent = MappingWindow.instance.get_tracked_widget(data["parent"])
            if parent is not None:
                parent.layout().addWidget(widget)
            
        if "enabled" in data:
            self.setEnabled(widget, data["enabled"])

    def get_data(self, widget, extraData):
        for child in widget.children():
            if not(hasattr(child, "__json_data__") and child.objectName() != ""):
                continue
            child_data = getattr(child, "__json_data__")
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
            elif isinstance(child, FormList):
                extraData[child.objectName()] = []
                for row in child.rows:
                    subFormData = {}
                    self.get_data(row, subFormData)
                    extraData[child.objectName()].append(subFormData)

    def setEnabled(self, widget, enabled):                        
        for child in widget.children():
            if type(child) in [QLineEdit, QComboBox, QCheckBox, QPushButton]:
                child.setEnabled(enabled)
            elif isinstance(child, FormList):
                child.addButton.setEnabled(enabled)
                for row in child.rows:
                    row.parent().children()[-1].setEnabled(enabled)
                    self.setEnabled(row, enabled)

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