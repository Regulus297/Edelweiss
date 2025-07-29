from ui import LayoutCreator
from plugins import plugin_loadable
from PyQt5.QtWidgets import QLayout, QHBoxLayout, QVBoxLayout


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
