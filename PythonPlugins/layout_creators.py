from ui.json_widget_loader import LayoutCreator, JSONWidgetLoader
from plugins.plugin_loadable import plugin_loadable
from plugins.load_dependencies import load_dependencies
from network.network_manager import PyNetworkManager
from PyQt5.QtWidgets import QLayout, QHBoxLayout, QVBoxLayout
from PyQt5.QtCore import Qt, QTimer


@plugin_loadable
class QHBoxLayoutLayoutCreator(LayoutCreator):
    def __init__(self):
        super().__init__("QHBoxLayout")
    
    def create_layout(self, data) -> QLayout:
        return QHBoxLayout()


@plugin_loadable
class QVBoxLayoutLayoutCreator(LayoutCreator):
    def __init__(self):
        super().__init__("QVBoxLayout")
    
    def create_layout(self, data) -> QLayout:
        return QVBoxLayout()
