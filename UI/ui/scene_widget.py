from PyQt5.QtWidgets import QWidget, QSizePolicy

from ui.main_window import MappingWindow


class SceneWidget(QWidget):
    def __init__(self, parent=None):
        super().__init__(parent)
        MappingWindow.scene_widgets[type(self).__name__] = type(self)
        self.setSizePolicy(QSizePolicy.Expanding, QSizePolicy.Expanding)
        self.init_ui()

    def init_ui(self):
        ...
