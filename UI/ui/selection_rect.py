from PyQt5.QtCore import QRect, QRectF, QPointF, Qt
from PyQt5.QtWidgets import QGraphicsItem

from ui.shape_item import ShapeItem


class SelectionRect(ShapeItem):
    selection_rects = {}

    def __init__(self, x, y, width, height, parent, _id):
        super().__init__(-10000000, width, height, parent, {
            "type": "rectangle",
            "x": 0,
            "y": 0,
            "width": width,
            "height": height,
            "color": "#6d9eed",
            "fill": "#446d9eed",
            "depth": -10000000,
            "thickness": 0.5
        })

        self.selectable = True
        self.x = x
        self.y = y
        self.setPos(self.x, self.y)
        self._width = width
        self._height = height
        self.id = _id
        self.edge_margin = 2
        self.resize_x = False
        self.resize_y = False

        self.setAcceptHoverEvents(True)

        self.offsetX = self.parent.x() - self.x
        self.offsetY = self.parent.y() - self.y

        self.setFlag(QGraphicsItem.ItemIsSelectable)
        self.setFlag(QGraphicsItem.ItemIsMovable)
        self.setFlag(QGraphicsItem.ItemSendsGeometryChanges)

        self.edge = None
        self.resizing = False
        self.start = None
        self.start_pos = None
        self.start_width = None
        self.start_height = None
        self.tags = []

        self.doubleClicking = False
        self.shiftClicked = False

    def addTag(self, tag):
        self.tags.append(tag)
        if tag not in SelectionRect.selection_rects:
            SelectionRect.selection_rects[tag] = []
        SelectionRect.selection_rects[tag].append(self)

    def removeTag(self, tag):
        if tag in self.tags:
            self.tags.remove(tag)
        if tag in SelectionRect.selection_rects and self in SelectionRect.selection_rects[tag]:
            SelectionRect.selection_rects[tag].remove(self)

    def mousePressEvent(self, event):
        if not bool(self.selectable):
            event.ignore()
            return
        if event.modifiers() & Qt.ShiftModifier:
            if self.doubleClicking:
                self.setSelected(True)
            else:
                self.setSelected(not self.isSelected())
            self.shiftClicked = True
            event.accept()
            return
        if self.edge:
            self.start = event.scenePos()
            self.start_pos = self.pos()
            self.start_width = self.width
            self.start_height = self.height
            self.resizing = True
        super().mousePressEvent(event)

    def mouseDoubleClickEvent(self, event):
        self.doubleClicking = True
        for tag in self.tags:
            for rect in SelectionRect.selection_rects[tag]:
                rect.setSelected(True)
        super().mouseDoubleClickEvent(event)

    def mouseMoveEvent(self, event):
        if self.resizing and self.edge is not None:
            diff = event.scenePos() - self.start
            diff.setX(round(diff.x()/8) * 8)
            diff.setY(round(diff.y()/8) * 8)

            if diff.x() != 0:
                if self.edge == "L":
                    self.setX(self.start_pos.x() + diff.x())
                    self.width = self.start_width - diff.x()
                    self.width = round(self.width)
                    self.parent.onSelectionResized(self.start_width, self.start_height, -diff.x(), 0, self.id)
                elif self.edge == "R":
                    self.width = self.start_width + diff.x()
                    self.width = round(self.width)
                    self.parent.onSelectionResized(self.start_width, self.start_height, diff.x(), 0, self.id)
            if diff.y() != 0:
                if self.edge == "T":
                    self.setY(self.start_pos.y() + diff.y())
                    self.height = self.start_height - diff.y()
                    self.height = round(self.height)
                    self.parent.onSelectionResized(self.start_width, self.start_height, 0, -diff.y(), self.id)
                elif self.edge == "B":
                    self.height = self.start_height + diff.y()
                    self.height = round(self.height)
                    self.parent.onSelectionResized(self.start_width, self.start_height, 0, diff.y(), self.id)
        else:
            super().mouseMoveEvent(event)


    def mouseReleaseEvent(self, event):
        self.resizing = False
        if not self.doubleClicking and not self.shiftClicked:
            super().mouseReleaseEvent(event)
        self.doubleClicking = False
        self.shiftClicked = False

    def itemChange(self, change, value):
        if change == QGraphicsItem.ItemSelectedChange:
            if not bool(self.selectable):
                self.parent.onSelectionChanged(False, self.id)
                return False
            self.parent.onSelectionChanged(value == 1, self.id)
        if change == QGraphicsItem.ItemPositionChange:
            if not bool(self.selectable):
                return self.pos()
            grid = 8
            x = value.x() - self.pos().x()
            y = value.y() - self.pos().y()
            snapped_x = round(x / grid) * grid
            snapped_y = round(y / grid) * grid
            self.parent.selectionMoved(self.id, QPointF(snapped_x, snapped_y))
            return QPointF(self.pos().x() + snapped_x, self.pos().y() + snapped_y)
        return super().itemChange(change, value)

    def boundingRect(self):
        return QRectF(0, 0, self.width, self.height)

    def hoverMoveEvent(self, event):
        pos = event.pos()
        if not self.isSelected():
            self.edge = None
            self.setCursor(Qt.OpenHandCursor)
            return
        if abs(pos.x() - self.width) < self.edge_margin and self.resize_x:
            self.edge = "R"
            self.setCursor(Qt.SizeHorCursor)
        elif abs(pos.x()) < self.edge_margin and self.resize_x:
            self.edge = "L"
            self.setCursor(Qt.SizeHorCursor)
        elif abs(pos.y() - self.height) < self.edge_margin and self.resize_y:
            self.edge = "B"
            self.setCursor(Qt.SizeVerCursor)
        elif abs(pos.y()) < self.edge_margin and self.resize_y:
            self.edge = "T"
            self.setCursor(Qt.SizeVerCursor)
        else:
            self.edge = None
            self.setCursor(Qt.OpenHandCursor)

    def paint(self, painter, option, widget = ...):
        if not self.isSelected():
            return
        super().paint(painter, option, widget)

    @property
    def width(self):
        return self._width

    @width.setter
    def width(self, value):
        self._width = value
        if len(self.shapes) > 0:
            self.shapes[0]["width"] = value

    @property
    def height(self):
        return self._height

    @height.setter
    def height(self, value):
        self._height = value
        if len(self.shapes) > 0:
            self.shapes[0]["height"] = value
