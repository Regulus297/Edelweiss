from ui.json_widget_loader import WidgetCreator, JSONWidgetLoader
from plugins.plugin_loadable import plugin_loadable
from plugins.load_dependencies import load_dependencies
from PyQt5.QtWidgets import QWidget, QSplitter, QPushButton
from PyQt5.QtCore import Qt, QTimer


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

        return widget