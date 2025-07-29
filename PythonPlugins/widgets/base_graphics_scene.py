import math

from PyQt5.QtCore import QLine
from PyQt5.QtGui import QColor, QPen
from PyQt5.QtWidgets import QGraphicsScene



class BaseGraphicsScene(QGraphicsScene):
    def __init__(self, parent=None):
        super().__init__(parent)

        self._color_background = QColor("#222222")
        self._scene_width, self._scene_height = 64000, 64000
        self.setScene(self._scene_width, self._scene_height)

        self.setBackgroundBrush(self._color_background)

    @property
    def backgroundColor(self):
        return self._color_background

    @backgroundColor.setter
    def backgroundColor(self, value):
        self._color_background = QColor(value)
        self.setBackgroundBrush(self._color_background)

    def setScene(self, width, height):
        self.setSceneRect(-width//2, -height//2, width, height)


