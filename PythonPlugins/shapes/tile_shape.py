from ui import ShapeRenderer
from plugins import plugin_loadable
from PyQt5.QtGui import QPainterPath, QPainter, QBrush, QColor
from PyQt5.QtCore import Qt


@plugin_loadable
class TileShape(ShapeRenderer):
    def __init__(self):
        super().__init__("tiles")

    def draw(self, painter, parent, data):
        pen = parent.get_pen(data)
        path = QPainterPath()
        path.addRect(0, 0, 16, 16)

        print(data["tileData"])

        painter.setPen(pen)

        painter.drawPath(path.simplified())
        painter.setRenderHint(QPainter.SmoothPixmapTransform, False)