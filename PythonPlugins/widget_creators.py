from ui.json_widget_loader import WidgetCreator, JSONWidgetLoader
from plugins.plugin_loadable import plugin_loadable
from plugins.load_dependencies import load_dependencies
from network.network_manager import PyNetworkManager
from PyQt5.QtWidgets import QWidget, QSplitter, QPushButton
from PyQt5.QtCore import Qt, QTimer
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
            widget.clicked.connect(lambda: PyNetworkManager.send_packet(data["onclick"], json.dumps({
                "id": widget.objectName()
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
        
        if "onCurrentItemChanged" in data.keys():
            def send_packet(curr, prev):
                send_data = json.dumps({
                    "id": widget.objectName(),
                    "prev": prev.text() if prev is not None else "",
                    "curr": curr.text(),
                    "prevRow": widget.row(prev) if prev is not None else -1,
                    "currRow": widget.row(curr)
                })
                PyNetworkManager.send_packet(data["onCurrentItemChanged"], send_data)

            widget.currentItemChanged.connect(send_packet)


        return widget
    

@plugin_loadable
class QWidgetWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QWidget")

    def create_widget(self, data, parent=None) -> QWidget:
        return QWidget(parent)
