import math

from PyQt5.QtCore import QLine
from PyQt5.QtGui import QColor, QPen
from PyQt5.QtWidgets import QGraphicsScene



class MappingScene(QGraphicsScene):
    def __init__(self, parent=None):
        super().__init__(parent)

        self.gridSize = 20
        self._color_background = QColor("#222222")
        self._color_light = QColor("#2f2f2f")
        self._color_dark = QColor("#292929")
        self._pen_light = QPen(self._color_light)
        self._pen_light.setWidth(1)
        self._pen_dark = QPen(self._color_dark)
        self._pen_dark.setWidth(2)
        self._scene_width, self._scene_height = 64000, 64000

        self.setBackgroundBrush(self._color_background)

    def setScene(self, width, height):
        self.setSceneRect(-width//2, -height//2, width, height)


    def drawBackground(self, painter, rect):
        super().drawBackground(painter, rect)

