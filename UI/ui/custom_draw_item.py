import json

from PyQt5.QtCore import QPoint, QRectF, Qt
from PyQt5.QtGui import QPainterPath, QPen, QPainter, QColor, QBrush
from PyQt5.QtWidgets import QGraphicsItem, QGraphicsView, QGraphicsScene

from network import PyNetworkManager
from plugins import get_event_data, DefaultEncoder


class CustomDrawItem(QGraphicsItem):
    shapes = {}

    def __init__(self, data):
        super().__init__()
        self.data = data
        self.shapes = data["shapes"]
        self.width = data["width"]
        self.height = data["height"]
        self.name = data["name"]
        self.setPos(QPoint(data["x"], data["y"]))
        if "zIndex" in data:
            self.setZValue(data["zIndex"])

        if "selectable" in data:
            self.setFlag(QGraphicsItem.ItemIsSelectable, data["selectable"])

        if "focusable" in data:
            self.setFlag(QGraphicsItem.ItemIsFocusable, data["focusable"])
            if not data["focusable"]:
                self.setAcceptedMouseButtons(Qt.NoButton)

        self.onMousePressed = lambda x, y: None
        if "onMousePressed" in data:
            netcode, extraData = get_event_data(data["onMousePressed"])
            self.onMousePressed = lambda x, y: PyNetworkManager.send_packet(netcode, json.dumps({
                "x": x,
                "y": y,
                "name": self.name,
                "type": "press",
                "extraData": extraData,
                "item": self.data
            }, cls=DefaultEncoder))

        self.onMouseMoved = lambda x, y: None
        if "onMouseMoved" in data:
            netcode, extraData = get_event_data(data["onMouseMoved"])
            self.onMouseMoved = lambda x, y: PyNetworkManager.send_packet(netcode, json.dumps({
                "x": x,
                "y": y,
                "name": self.name,
                "type": "move",
                "extraData": extraData,
                "item": self.data
            }, cls=DefaultEncoder))

        self.onMouseReleased = lambda x, y: None
        if "onMouseReleased" in data:
            netcode, extraData = get_event_data(data["onMouseReleased"])
            self.onMouseReleased = lambda x, y: PyNetworkManager.send_packet(netcode, json.dumps({
                "x": x,
                "y": y,
                "name": self.name,
                "type": "release",
                "extraData": extraData,
                "item": self.data
            }, cls=DefaultEncoder))

        self.shapeRenderers = []
        for shape in self.shapes:
            self.shapeRenderers.append(type(CustomDrawItem.shapes[shape["type"]])(self, shape))

    def paint(self, painter, option, widget = ...):
        for shape in self.shapeRenderers:
            if "visible" in shape.data and not shape.data["visible"]:
                continue
            shape.draw(painter)

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

    def refresh(self, data):
        self.data.update(data)
        self.width = self.data["width"]
        self.height = self.data["height"]
        self.name = self.data["name"]
        self.setPos(QPoint(self.data["x"], self.data["y"]))
        self.update()

    def mousePressEvent(self, event):
        super().mousePressEvent(event)
        self.onMousePressed(event.pos().x(), event.pos().y())

    def mouseMoveEvent(self, event):
        super().mouseMoveEvent(event)
        self.onMouseMoved(event.pos().x(), event.pos().y())

    def mouseReleaseEvent(self, event):
        super().mouseReleaseEvent(event)
        self.onMouseReleased(event.pos().x(), event.pos().y())