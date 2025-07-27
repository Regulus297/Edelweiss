from ui.shape_renderer import ShapeRenderer
from plugins.plugin_loadable import plugin_loadable
from PyQt5.QtGui import QPainterPath, QPainter, QBrush, QColor
from PyQt5.QtCore import Qt


@plugin_loadable
class RectangleShape(ShapeRenderer):
    def __init__(self):
        super().__init__("rectangle")

    def draw(self, painter, parent, data):
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