from ui import WidgetCreator, JSONWidgetLoader, FormList, ModifiableCombobox
from plugins import plugin_loadable, load_dependencies, get_event_data
from network import PyNetworkManager
from PyQt5.QtWidgets import QWidget, QSplitter, QPushButton, QLineEdit, QComboBox, QCheckBox, QLabel
from PyQt5.QtCore import Qt, QTimer
from PyQt5.QtGui import QIntValidator, QDoubleValidator
import json


@plugin_loadable
class QSplitterWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QSplitter")
    
    def create_widget(self, data, parent=None) -> QWidget:
        widget = QSplitter(Qt.Horizontal if data["horizontal"] else Qt.Vertical, parent)

        for child in data["children"]:
            widget.addWidget(JSONWidgetLoader.init_widget(child, None))

        def resize():
            if "sizes" in data.keys():
                sizes = data["sizes"]
                total = sum(sizes)
                weights = [int((widget.width() if data["horizontal"] else widget.height()) * size/total) for size in sizes]
                widget.setSizes(weights)
            
        QTimer.singleShot(0, resize)

        return widget
    
@plugin_loadable
class QPushButtonWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QPushButton")
    
    def create_widget(self, data, parent=None) -> QWidget:
        widget = QPushButton(data["text"], parent)

        if "onclick" in data.keys():
            netcode, extraData = get_event_data(data["onclick"])
            widget.clicked.connect(lambda: PyNetworkManager.send_packet(netcode, json.dumps({
                "id": widget.objectName(),
                "extraData": extraData
            })))

        return widget
    

@load_dependencies("widgets/base_graphics_scene.py", "widgets/zoomable_view.py")
@plugin_loadable
class ZoomableViewWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("ZoomableView")

    def create_widget(self, data, parent=None) -> QWidget:
        widget = ZoomableView(parent)
        if "bgcolor" in data.keys():
            widget.grScene.backgroundColor = data["bgcolor"]

        if "onMouseMoved" in data:
            netcode, extraData = get_event_data(data["onMouseMoved"])
            widget.onMouseMoved = lambda x, y: PyNetworkManager.send_packet(netcode, json.dumps({
                "id": widget.objectName(),
                "x": x,
                "y": y,
                "extraData": extraData
            }))

        return widget
    

@load_dependencies("widgets/resizing_list.py", "common_code.py")
@plugin_loadable
class ResizingListWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("ResizingList")

    def create_widget(self, data, parent=None) -> QWidget:
        widget = ResizingList(parent)
        widget.setUpdatesEnabled(False)
        if "items" in data.keys():
            keys = data["keys"] if "keys" in data else range(len(data["items"]))

            for key, item in zip(keys, data["items"]):
                widget.addKeyValuePair(key, item)
        widget.setUpdatesEnabled(True)

        if "currentRow" in data.keys():
            widget.setCurrentRow(value(data["currentRow"]))
        
        if "onCurrentItemChanged" in data.keys():
            netcode, extraData = get_event_data(data["onCurrentItemChanged"])
            def send_packet(curr, prev):
                send_data = json.dumps({
                    "id": widget.objectName(),
                    "prev": prev.text() if prev is not None else "",
                    "curr": curr.text() if curr is not None else "",
                    "prevID": widget.keys[widget.row(prev)] if prev is not None else None,
                    "currID": widget.keys[widget.row(curr)] if curr is not None else None,
                    "extraData": extraData
                })
                PyNetworkManager.send_packet(netcode, send_data)

            widget.currentItemChanged.connect(send_packet)

        if "onItemDoubleClicked" in data:
            netcode1, extraData1 = get_event_data(data["onItemDoubleClicked"])
            widget.itemDoubleClicked.connect(lambda material: PyNetworkManager.send_packet(netcode1, json.dumps({
                "id": widget.objectName(),
                "item": material.text(),
                "itemID": widget.keys[widget.row(material)],
                "extraData": extraData1
            })))


        return widget
    
    def refresh_widget(self, widget):
        widget.clear()
        data = getattr(widget, "__json_data__")
        if "items" in data.keys():
            keys = data["keys"] if "keys" in data else list(range(len(data["items"])))

            widget.keys = value(keys)
            widget.addItems(value(data["items"]))

        if "currentRow" in data.keys():
            widget.setCurrentRow(value(data["currentRow"]))
    

@plugin_loadable
class QWidgetWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QWidget")

    def create_widget(self, data, parent=None) -> QWidget:
        return QWidget(parent)


@load_dependencies("validators.py", load_types = True)
@plugin_loadable
class QLineEditWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QLineEdit")

    def create_widget(self, data, parent=None) -> QWidget:
        lineEdit = QLineEdit(parent)
        default = data["default"] if "default" in data.keys() else ""
        lineEdit.setText(str(default))
        if "dataType" in data:
            if data["dataType"] == "int":
                minimum = data["min"] if "min" in data.keys() else None
                maximum = data["max"] if "max" in data.keys() else None
                if default == "":
                    lineEdit.setText("0")
                if minimum is not None and maximum is not None:
                    lineEdit.setValidator(QIntValidator(lineEdit))
                    self.clamp_line_edit_int(lineEdit, minimum, maximum)
                    lineEdit.editingFinished.connect(lambda: self.clamp_line_edit_int(lineEdit, minimum, maximum))
                else:
                    lineEdit.setValidator(QIntValidator(lineEdit))
            elif data["dataType"] == "float":
                minimum = data["min"] if "min" in data.keys() else None
                maximum = data["max"] if "max" in data.keys() else None
                decimals = data["decimals"] if "decimals" in data.keys() else 2
                if default == "":
                    lineEdit.setText("0.0")
                if minimum is not None and maximum is not None:

                    lineEdit.setValidator(QDoubleValidator(lineEdit))
                    self.clamp_line_edit_float(lineEdit, minimum, maximum, decimals)
                    lineEdit.editingFinished.connect(lambda: self.clamp_line_edit_float(lineEdit, minimum, maximum, decimals))
                else:
                    lineEdit.setValidator(QDoubleValidator(lineEdit))

        if "validator" in data:
            validator = Validator.init_validator(data["validator"])
            lineEdit.editingFinished.connect(lambda: validator.editingFinished(lineEdit))
            lineEdit.__custom_validator__ = validator

        if "onTextChanged" in data:
            netcode, extraData = get_event_data(data["onTextChanged"])
            lineEdit.textChanged.connect(lambda text: PyNetworkManager.send_packet(netcode, json.dumps({
                "id": lineEdit.objectName(),
                "text": text,
                "extraData": extraData
            })))

        return lineEdit
    
    def clamp_line_edit_int(self, widget, low, high):
        widget.clearFocus()
        try:
            widget.setText(str(min(high, max(low, int(widget.text())))))
        except:
            return
        
    def clamp_line_edit_float(self, widget, low, high, decimals):
        try:
            widget.setText(str(min(high, max(low, round(float(widget.text()), decimals)))))
        except:
            return
        

@plugin_loadable
class QComboBoxWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QComboBox")

    def create_widget(self, data, parent=None) -> QWidget:
        if data.get("modifiable"):
            return self._create_modifiable_combobox(data, parent)
        return self._create_default_combobox(data, parent)
    
    def _create_default_combobox(self, data, parent):
        combobox = QComboBox(parent)
        keyed = False
        if isinstance(data["items"], dict):
            keyed = True
            for key, value in data["items"].items():
                combobox.addItem(str(key), value)
        else:    
            combobox.addItems([str(i) for i in data["items"]])
        combobox.setEditable("editable" in data and data["editable"])
        if "default" in data:
            if not keyed:
                combobox.setCurrentText(str(data["default"]))
                return combobox
            
            found = False
            for i in range(combobox.count()):
                if combobox.itemData(i) == data["default"]:
                    combobox.setCurrentIndex(i)
                    found = True
                    break
            if not found:
                combobox.setCurrentText(str(data["default"]))
        return combobox
    
    def _create_modifiable_combobox(self, data, parent):
        combobox = ModifiableCombobox(data["defaults"] if "defaults" in data else data["items"], parent)
        if "defaults" in data:
            for item in data["items"]:
                QTimer.singleShot(0, lambda: combobox.addItem(item))

        if "canEditDefaults" in data:
            combobox.canEditDefaults = bool(data["canEditDefaults"])

        if "itemEdited" in data:
            netcode, extraData = get_event_data(data["itemEdited"])
            combobox.itemEdited.connect(lambda old, new: PyNetworkManager.send_packet(netcode, json.dumps({
                "id": combobox.objectName(),
                "old": old,
                "new": new,
                "extraData": extraData
            })))
        if "itemChanged" in data:
            netcode1, extraData1 = get_event_data(data["itemChanged"])
            combobox.itemChanged.connect(lambda item: PyNetworkManager.send_packet(netcode1, json.dumps({
                "id": combobox.objectName(),
                "item": item,
                "extraData": extraData1
            })))
        if "itemAdded" in data:
            netcode2, extraData2 = get_event_data(data["itemAdded"])
            combobox.itemAdded.connect(lambda item: PyNetworkManager.send_packet(netcode2, json.dumps({
                "id": combobox.objectName(),
                "item": item,
                "extraData": extraData2
            })))
        if "itemRemoved" in data:
            netcode3, extraData3 = get_event_data(data["itemRemoved"])
            combobox.itemRemoved.connect(lambda item: PyNetworkManager.send_packet(netcode3, json.dumps({
                "id": combobox.objectName(),
                "item": item,
                "extraData": extraData3
            })))

        return combobox

@plugin_loadable
class QCheckBoxWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QCheckBox")

    def create_widget(self, data, parent=None) -> QWidget:
        checkbox = QCheckBox(parent)
        if "default" in data.keys():
            checkbox.setChecked(bool(data["default"]))
        return checkbox


@plugin_loadable
class QLabelWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QLabel")

    def create_widget(self, data, parent=None) -> QWidget:
        label = QLabel("" if "text" not in data else data["text"], parent)
        if "alignment" in data:
            label.setAlignment(data["alignment"])
        return label


@load_dependencies("common_code.py")
@plugin_loadable
class FormListWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("FormList")

    def create_widget(self, data, parent=None) -> QWidget:
        widgetJSON = data["widgetJSON"]
        formList = FormList(lambda: JSONWidgetLoader.init_widget(copyJSON(widgetJSON), None))

        if "items" in data:
            for childJSON in data["items"]:
                formList.addRow(JSONWidgetLoader.init_widget(childJSON, None))

        return formList

