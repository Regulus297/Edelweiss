from ui import ShapeRenderer
from plugins import plugin_loadable
from PyQt5.QtGui import QPainterPath, QPainter, QBrush, QColor
from PyQt5.QtCore import Qt


@plugin_loadable
class RectangleShape(ShapeRenderer):
    def __init__(self, parent=None, data=None):
        super().__init__("rectangle", parent, data)

    def draw(self, painter):
        parent = self.parent
        data = self.data

        pen = parent.get_pen(data)
        path = QPainterPath()
        x, y, width, height = parent.get_dimensions(data["x"], data["y"], data["width"], data["height"])
        path.addRect(x, y, width, height)

        brush = Qt.NoBrush
        if "fill" in data.keys():
            brush = QBrush()
            brush.setColor(QColor(data["fill"]))
        
        painter.setBrush(brush)
        painter.setPen(pen)

        painter.drawPath(path.simplified())
        painter.setRenderHint(QPainter.SmoothPixmapTransform, False)