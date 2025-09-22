import json

from PyQt5.QtCore import QPoint, QRectF, Qt, QTimer
from PyQt5.QtGui import QPainterPath, QPen, QPainter, QColor, QBrush, QPixmap
from PyQt5.QtWidgets import QGraphicsItem, QGraphicsView, QGraphicsScene

from network import PyNetworkManager
from plugins import get_event_data, DefaultEncoder
from ui.shape_item import ShapeItem


class CustomDrawItem(QGraphicsItem):
    shapes = {}

    def __init__(self, data, parent=None):
        super().__init__(parent)
        self.data = data
        self.shapes = data["shapes"]
        self.width = data["width"]
        self.height = data["height"]
        self.name = data["name"]
        self.setPos(QPoint(data["x"], data["y"]))

        if "rotation" in data:
            self.setRotation(data["rotation"])

        if "zIndex" in data:
            self.setZValue(data["zIndex"])

        if "selectable" in data:
            self.setFlag(QGraphicsItem.ItemIsSelectable, data["selectable"])

        if "focusable" in data:
            self.setFlag(QGraphicsItem.ItemIsFocusable, data["focusable"])
            if not data["focusable"]:
                self.setAcceptedMouseButtons(Qt.NoButton)

        self.onMousePressed = lambda x, y, b: None
        if "onMousePressed" in data:
            netcode, extraData = get_event_data(data["onMousePressed"])
            self.onMousePressed = lambda x, y, b: PyNetworkManager.send_packet(netcode, json.dumps({
                "x": x,
                "y": y,
                "name": self.name,
                "type": "press",
                "button": b,
                "extraData": extraData,
                "item": self.data
            }, cls=DefaultEncoder))

        self.onMouseMoved = lambda x, y, b: None
        if "onMouseMoved" in data:
            netcode1, extraData1 = get_event_data(data["onMouseMoved"])
            self.onMouseMoved = lambda x, y, b: PyNetworkManager.send_packet(netcode1, json.dumps({
                "x": x,
                "y": y,
                "name": self.name,
                "type": "move",
                "button": b,
                "extraData": extraData1,
                "item": self.data
            }, cls=DefaultEncoder))

        self.onMouseReleased = lambda x, y, b: None
        if "onMouseReleased" in data:
            netcode2, extraData2 = get_event_data(data["onMouseReleased"])
            self.onMouseReleased = lambda x, y, b: PyNetworkManager.send_packet(netcode2, json.dumps({
                "x": x,
                "y": y,
                "name": self.name,
                "type": "release",
                "button": b,
                "extraData": extraData2,
                "item": self.data
            }, cls=DefaultEncoder))


        self.shapeRenderers = []
        self.items = {}
        self.setFlag(QGraphicsItem.ItemSendsGeometryChanges, True)
        for shape in self.shapes:
            self.addShapeDelayed(shape)

    def addShapeDelayed(self, shape):
        QTimer.singleShot(0, lambda: self.addShape(shape))

    def addShape(self, shape):
        depth = shape["depth"] if "depth" in shape else 0
        if depth not in self.items:
            item = ShapeItem(depth, self.width, self.height, self, shape)
            self.items[depth] = item
            self.scene().addItem(item)
            item.setTransform(self.sceneTransform())
        else:
            item = self.items[depth]
        item.addShape(shape)

    def paint(self, painter, option, widget = ...):
        if "opacity" in self.data:
            painter.setOpacity(float(self.data["opacity"]))

    def boundingRect(self):
        return QRectF(0, 0, self.width, self.height)

    def get_pen(self, shape):
        pen = QPen()
        pen.setColor(QColor(shape["color"]))
        pen.setWidthF(float(shape["thickness"]))
        return pen

    def itemChange(self, change, value):
        if change in (QGraphicsItem.ItemPositionHasChanged, QGraphicsItem.ItemTransformHasChanged):
            trans = self.sceneTransform()
            for i in self.items.values():
                i.setTransform(trans)
        return super().itemChange(change, value)

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

    def refresh(self, data, update_shapes=False):
        self.data.update(data)
        self.width = self.data["width"]
        self.height = self.data["height"]
        self.name = self.data["name"]
        self.setPos(QPoint(self.data["x"], self.data["y"]))

        if "rotation" in self.data:
            self.setRotation(self.data["rotation"])

        if update_shapes:
            self.clear()
            for shape in data["shapes"]:
                self.addShape(shape)

        self.update()

    def clear(self):
        for _, item in self.items.items():
            item.shapes.clear()
            item.shapeRenderers.clear()

    def mousePressEvent(self, event):
        super().mousePressEvent(event)
        if "focusable" not in self.data or not self.data["focusable"]:
            return
        event.accept()
        self.onMousePressed(event.pos().x(), event.pos().y(), event.button())

    def mouseMoveEvent(self, event):
        super().mouseMoveEvent(event)
        self.onMouseMoved(event.pos().x(), event.pos().y(), event.button())

    def mouseReleaseEvent(self, event):
        super().mouseReleaseEvent(event)
        self.onMouseReleased(event.pos().x(), event.pos().y(), event.button())