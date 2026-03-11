from ui import JSONWidgetLoader, MainWindow, WidgetMethod
from plugins import plugin_loadable
from interop import SyncableProperty
from PyQt5.QtWidgets import QFileDialog
import json


@plugin_loadable
class OpenPopupFunc:
    widgets = []

    def __init__(self):
        event = SyncableProperty("Edelweiss.OnOpenWidget", False).get()
        event += self.open

    def open(self, data):
        widget = JSONWidgetLoader.init_widget(json.loads(data))
        widget.show()
        widget.setObjectName("popup")
        widget.setStyleSheet(MainWindow.stylesheet)
        OpenPopupFunc.widgets.append(widget)


@plugin_loadable
class OpenFileDialogFunc:
    def __init__(self):
        event = SyncableProperty("Edelweiss.OnOpenFileDialog", False).get()
        event += self.open

    def open(self, data):
        data = json.loads(data)
        file = data["file"]
        directory = data.get("path", "")
        _filter = data.get("pattern", "")
        existing = data.get("mode", "load") == "load"
        
        if file:
            if existing:
                path, pattern = QFileDialog.getOpenFileName(directory = directory, filter = _filter)
            else:
                path, pattern = QFileDialog.getSaveFileName(directory = directory, filter = _filter)
        else:
            path, pattern = QFileDialog.getExistingDirectory(directory = directory), ""
        
        if path == "":
            return
        
        WidgetMethod.create(None, None, data, "submit", {"path": lambda: path, "pattern": lambda: pattern})._invoke()