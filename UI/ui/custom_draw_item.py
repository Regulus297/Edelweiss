from PyQt5.QtCore import QPoint, QRectF, Qt
from PyQt5.QtGui import QPainterPath, QPen, QPainter, QColor, QBrush
from PyQt5.QtWidgets import QGraphicsItem, QGraphicsView, QGraphicsScene


class CustomDrawItem(QGraphicsItem):
    shapes = {}

    def __init__(self, data):
        super().__init__()
        self.shapes = data["shapes"]
        self.width = data["width"]
        self.height = data["height"]
        self.name = data["name"]
        self.setPos(QPoint(data["x"], data["y"]))



    def paint(self, painter, option, widget = ...):
        for shape in self.shapes:
            CustomDrawItem.shapes[shape["type"]].draw(painter, self, shape)

    def boundingRect(self):
        return QRectF(0, 0, self.width, self.height)


    def get_pen(self, shape):
        pen = QPen()
        pen.setColor(QColor(shape["color"]))
        pen.setWidthF(float(shape["thickness"]))
        return pen

    def get_dimensions(self, x, y, width, height):
        if type(x) == float:
            x = int(x * self.width)
        if type(y) == float:
            y = int(y * self.height)
        if type(width) == float:
            width = int(width * self.width)
        if type(height) == float:
            height = int(height * self.height)

        return x, y, width, height
