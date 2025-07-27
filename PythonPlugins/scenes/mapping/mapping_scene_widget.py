from PyQt5.QtCore import Qt, QEvent
from PyQt5.QtGui import QPainter, QMouseEvent
from PyQt5.QtWidgets import QGraphicsView, QVBoxLayout, QSizePolicy
from plugins.plugin_loadable import plugin_loadable
from plugins.load_dependencies import load_dependencies
from ui.scene_widget import SceneWidget


@load_dependencies("mapping_scene.py", "mapping_view.py")
@plugin_loadable
class MappingSceneWidget(SceneWidget):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.layout = QVBoxLayout(self)
        self.layout.setContentsMargins(0, 0, 0, 0)
        self.setSizePolicy(QSizePolicy.Expanding, QSizePolicy.Expanding)
        self.layout.addWidget(MappingView(self))