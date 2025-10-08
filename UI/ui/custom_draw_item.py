import json

from PyQt5.QtCore import QPoint, QRectF, Qt, QTimer
from PyQt5.QtGui import QPainterPath, QPen, QPainter, QColor, QBrush, QPixmap
from PyQt5.QtWidgets import QGraphicsItem, QGraphicsView, QGraphicsScene

from network import PyNetworkManager
from plugins import get_event_data, DefaultEncoder
from ui.selection_rect import SelectionRect
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
            self.setFlag(QGraphicsItem.ItemIsSelectable, bool(data["selectable"]))

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

        self.selections = []
        self.onSelectionMoved = lambda x, y, i: None
        if "onSelectionMoved" in data:
            netcode3, extraData3 = get_event_data(data["onSelectionMoved"])
            self.onSelectionMoved = lambda x, y, i: PyNetworkManager.send_packet(netcode3, json.dumps({
                "x": x,
                "y": y,
                "name": self.name,
                "index": i,
                "extraData": extraData3
            }, cls=DefaultEncoder))

        self.onSelectionChanged = lambda selected, index: None
        if "onSelectionChanged" in data:
            netcode4, extraData4 = get_event_data(data["onSelectionChanged"])
            self.onSelectionChanged = lambda selected, index: PyNetworkManager.send_packet(netcode4, json.dumps({
                "name": self.name,
                "selected": selected,
                "index": index,
                "extraData": extraData4
            }, cls=DefaultEncoder))

        self.onSelectionRightClicked = lambda selected, index, x, y: None
        if "onSelectionChanged" in data:
            netcode5, extraData5 = get_event_data(data["onSelectionRightClicked"])
            self.onSelectionRightClicked = lambda selected, index, x, y: PyNetworkManager.send_packet(netcode5, json.dumps({
                "name": self.name,
                "selected": selected,
                "index": index,
                "x": x,
                "y": y,
                "extraData": extraData5
            }, cls=DefaultEncoder))

        self.onSelectionResized = lambda x1, y1, x2, y2, i: None
        if "onSelectionResized" in data:
            netcode6, extraData6 = get_event_data(data["onSelectionResized"])
            self.onSelectionResized = lambda x1, y1, x2, y2, i: PyNetworkManager.send_packet(netcode6, json.dumps({
                "name": self.name,
                "deltaWidth": x2,
                "deltaHeight": y2,
                "oldWidth": x1,
                "oldHeight": y1,
                "index": i,
                "extraData": extraData6
            }, cls=DefaultEncoder))

        if "selection" in data:
            j = 0
            self.setFlag(QGraphicsItem.ItemIsSelectable, False)
            for selection in data["selection"]:
                rect = SelectionRect(selection["x"], selection["y"], selection["width"], selection["height"], self, j)
                rect.selectable = data["selectable"] if "selectable" in data else True
                rect.resize_x = selection["resizeX"] if "resizeX" in selection else False
                rect.resize_y = selection["resizeY"] if "resizeY" in selection else False
                if "tags" in selection:
                    for tag in selection["tags"]:
                        rect.addTag(tag)
                self.addRectDelayed(rect)
                j += 1



        self.items = {}
        self.shapesBySelection = {}
        self.setFlag(QGraphicsItem.ItemSendsGeometryChanges, True)
        self.clickButton = 0
        for shape in self.shapes:
            self.addShapeDelayed(shape)

    def addRectDelayed(self, rect):
        self.selections.append(rect)
        QTimer.singleShot(0, lambda: self.scene().addItem(rect))

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

    def selectionMoved(self, index, pos):
        self.onSelectionMoved(pos.x(), pos.y(), index)

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

        if "selection" in data:
            if len(data["selection"]) < len(self.selections):
                self.scene().clearSelection()
                for i in range(len(data["selection"]), len(self.selections)):
                    self.scene().removeItem(self.selections[i])
                del self.selections[len(data["selection"]):]

            for j in range(len(data["selection"])):
                selection = data["selection"][j]
                if j >= len(self.selections):
                    rect = SelectionRect(selection["x"], selection["y"], selection["width"], selection["height"], self, j)
                    rect.selectable = data["selectable"] if "selectable" in data else True
                    self.addRectDelayed(rect)
                else:
                    current: SelectionRect = self.selections[j]
                    current.setFlag(QGraphicsItem.ItemSendsGeometryChanges, False)
                    current.setPos(selection["x"], selection["y"])
                    current.setFlag(QGraphicsItem.ItemSendsGeometryChanges, True)
                    current.width = selection["width"]
                    current.height = selection["height"]
                    current.shapes[0]["width"] = selection["width"]
                    current.shapes[0]["height"] = selection["height"]





        if update_shapes:
            self.clear()
            for shape in data["shapes"]:
                self.addShape(shape)

        self.update()

    def clear(self):
        for _, item in self.items.items():
            item.shapes.clear()
            item.shapeRenderers.clear()

    def delete(self):
        for _, item in self.items.items():
            self.scene().removeItem(item)

        for selection in self.selections:
            self.scene().removeItem(selection)
        self.scene().removeItem(self)
        del self

    def mousePressEvent(self, event):
        super().mousePressEvent(event)
        if "focusable" not in self.data or not self.data["focusable"]:
            return
        event.accept()
        self.clickButton = event.button()
        self.onMousePressed(event.pos().x(), event.pos().y(), event.button())

    def mouseMoveEvent(self, event):
        super().mouseMoveEvent(event)
        self.onMouseMoved(event.pos().x(), event.pos().y(), self.clickButton)

    def mouseReleaseEvent(self, event):
        super().mouseReleaseEvent(event)
        self.clickButton = 0
        self.onMouseReleased(event.pos().x(), event.pos().y(), event.button())