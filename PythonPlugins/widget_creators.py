from ui import WidgetCreator, JSONWidgetLoader
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

        return widget
    

@load_dependencies("widgets/resizing_list.py")
@plugin_loadable
class ResizingListWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("ResizingList")

    def create_widget(self, data, parent=None) -> QWidget:
        widget = ResizingList(parent)
        if "items" in data.keys():
            for item in data["items"]:
                widget.addItem(item)

        if "currentRow" in data.keys():
            widget.setCurrentRow(int(data["currentRow"]))
        
        if "onCurrentItemChanged" in data.keys():
            netcode, extraData = get_event_data(data["onCurrentItemChanged"])
            def send_packet(curr, prev):
                send_data = json.dumps({
                    "id": widget.objectName(),
                    "prev": prev.text() if prev is not None else "",
                    "curr": curr.text() if curr is not None else "",
                    "prevRow": widget.row(prev) if prev is not None else -1,
                    "currRow": widget.row(curr) if curr is not None else -1,
                    "extraData": extraData
                })
                PyNetworkManager.send_packet(netcode, send_data)

            widget.currentItemChanged.connect(send_packet)


        return widget
    
    def refresh_widget(self, widget):
        widget.clear()
        data = getattr(widget, "__json_data__")
        if "items" in data.keys():
            for item in data["items"]:
                widget.addItem(item)

        if "currentRow" in data.keys():
            widget.setCurrentRow(int(data["currentRow"]))
    

@plugin_loadable
class QWidgetWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QWidget")

    def create_widget(self, data, parent=None) -> QWidget:
        return QWidget(parent)


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
        combobox = QComboBox(parent)
        combobox.addItems(data["items"])
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
        label = QLabel("" if "text" not in data.keys() else data["text"], parent)
        if "alignment" in data.keys():
            label.setAlignment(data["alignment"])
        return label
