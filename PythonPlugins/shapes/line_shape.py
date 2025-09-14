from ui import ShapeRenderer
from plugins import plugin_loadable, load_dependencies
from PyQt5.QtGui import QPainterPath, QPainter, QBrush, QColor
from PyQt5.QtCore import Qt, QPointF, QLineF


@load_dependencies("../common_code.py")
@plugin_loadable
class LineShape(ShapeRenderer):
    def __init__(self, parent=None, data=None):
        super().__init__("line", parent, data)

    def draw(self, painter):
        parent = self.parent
        data = self.data

        pen = parent.get_pen(data)
        painter.setPen(pen)
        x1, y1, x2, y2 = value(data["x1"]), value(data["y1"]), value(data["x2"]), value(data["y2"])

        painter.drawLine(QLineF(x1, y1, x2, y2))