from PyQt5.QtCore import QRectF
from PyQt5.QtWidgets import QGraphicsItem


class ShapeItem(QGraphicsItem):
    shapes = {}

    def __init__(self, depth, width, height, parent, *shapes):
        super().__init__()
        self.setZValue(-depth)
        self.parent = parent
        self.shapes = []
        self.shapeRenderers = []
        self.width = width
        self.height = height

        for shape in shapes:
            self.addShape(shape)

    def boundingRect(self):
        return QRectF(0, 0, self.width, self.height)

    def addShape(self, shape):
        self.shapes.append(shape)
        self.shapeRenderers.append(type(ShapeItem.shapes[shape["type"]])(self.parent, shape))

    def paint(self, painter, option, widget = ...):
        for shape in self.shapeRenderers:
            if "visible" in shape.data and not shape.data["visible"]:
                continue
            shape.draw(painter)