from PyQt5.QtCore import Qt, QEvent
from PyQt5.QtGui import QPainter, QMouseEvent
from PyQt5.QtWidgets import QGraphicsView, QSizePolicy
from plugins import load_dependencies


@load_dependencies("base_graphics_scene.py")
class ZoomableView(QGraphicsView):
    def __init__(self, parent=None):
        super().__init__(parent)
        self.trackedItems = {}
        self.trackedGraphicsItems = {}
        self.grScene = BaseGraphicsScene(self)

        self.initUI()

        self.setScene(self.grScene)
        self.setSizePolicy(QSizePolicy.Expanding, QSizePolicy.Expanding)
        self.setDragMode(QGraphicsView.ScrollHandDrag)
        self.scale_factor = 1.5
        self.min_zoom = 0.1
        self.max_zoom = 200
        self.current_zoom = 1.0
        self.setMouseTracking(True)

        self.onMouseMoved = lambda x, y: None

        self.pen_size = 5

    def mouseMoveEvent(self, event):
        super().mouseMoveEvent(event)
        pos = self.mapToScene(event.pos())
        self.onMouseMoved(pos.x(), pos.y())

    def wheelEvent(self, event):
        zoom_in = event.angleDelta().y() > 0

        pos_before = self.mapToScene(event.pos())
        if zoom_in:
            if self.current_zoom < self.max_zoom:
                self.scale(self.scale_factor, self.scale_factor)
                self.current_zoom *= self.scale_factor
        else:
            if self.current_zoom > self.min_zoom:
                self.scale(1 / self.scale_factor, 1 / self.scale_factor)
                self.current_zoom /= self.scale_factor

        size = 4 / self.current_zoom
        size = max(size, 1)
        size = min(size, 20)
        self.pen_size = size
        pos_after = self.mapToScene(event.pos())
        delta = pos_after - pos_before
        self.centerOn(self.mapToScene(self.viewport().rect().center()) - delta)

    def initUI(self):
        self.setRenderHints(QPainter.Antialiasing | QPainter.HighQualityAntialiasing | QPainter.TextAntialiasing)
        self.setOptimizationFlags(QGraphicsView.DontClipPainter | QGraphicsView.DontSavePainterState)
        self.setViewportUpdateMode(QGraphicsView.FullViewportUpdate)

        self.setHorizontalScrollBarPolicy(Qt.ScrollBarAlwaysOff)
        self.setVerticalScrollBarPolicy(Qt.ScrollBarAlwaysOff)
