from ui import ShapeRenderer
from plugins import plugin_loadable
from PyQt5.QtGui import QPainterPath, QPainter, QBrush, QColor
from PyQt5.QtCore import Qt


@plugin_loadable
class TileGhostShape(ShapeRenderer):
    def __init__(self, parent=None, data=None):
        super().__init__("tileGhost", parent, data)

    def draw(self, painter):
        parent = self.parent
        data = self.data

        coords = [(int(item.split(",")[0]), int(item.split(",")[1])) for item in data["coords"]]
        pen = parent.get_pen(data)
        path = QPainterPath()
        for coord in coords:
            path.addRect(coord[0] * 8, coord[1] * 8, 8, 8)

        painter.setPen(pen)
        painter.drawPath(path.simplified())
        painter.setRenderHint(QPainter.SmoothPixmapTransform, False)