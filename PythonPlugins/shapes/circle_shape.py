from ui import ShapeRenderer
from plugins import plugin_loadable
from PyQt5.QtGui import QPainterPath, QPainter, QBrush, QColor
from PyQt5.QtCore import Qt, QPointF


@plugin_loadable
class CircleShape(ShapeRenderer):
    def __init__(self, parent=None, data=None):
        super().__init__("circle", parent, data)

    def draw(self, painter):
        parent = self.parent
        data = self.data

        pen = parent.get_pen(data)
        path = QPainterPath()
        x, y, _, _ = parent.get_dimensions(data["x"], data["y"], 0, 0)
        path.addEllipse(QPointF(x, y), data["radius"], data["radius"])

        brush = Qt.NoBrush
        if "fill" in data.keys():
            brush = QBrush()
            brush.setColor(QColor(data["fill"]))
        
        painter.setBrush(brush)
        painter.setPen(pen)

        painter.drawPath(path.simplified())