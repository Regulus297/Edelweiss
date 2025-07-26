from PyQt5.QtCore import QPoint, QRectF
from PyQt5.QtGui import QPainterPath, QPen, QPainter, QColor
from PyQt5.QtWidgets import QGraphicsItem


class CustomDrawItem(QGraphicsItem):
    def __init__(self, data):
        super().__init__()
        self.shapes = data["shapes"]
        self.width = data["width"]
        self.height = data["height"]
        self.setPos(QPoint(data["x"], data["y"]))


    def paint(self, painter, option, widget = ...):
        for shape in self.shapes:
            pen = self.get_pen(shape)
            if shape["type"] == "rectangle":
                path = QPainterPath()
                path.addRect(self.x(), self.y(), self.width, self.height)
                painter.setPen(pen)
                painter.drawPath(path.simplified())
                painter.setRenderHint(QPainter.SmoothPixmapTransform, False)

    def boundingRect(self):
        return QRectF(self.x(), self.y(), self.width, self.height)


    def get_pen(self, shape):
        pen = QPen()
        pen.setColor(QColor(shape["color"]))
        pen.setWidthF(shape["width"])
        return pen
