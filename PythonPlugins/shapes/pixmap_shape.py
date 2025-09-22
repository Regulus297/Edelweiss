from ui import ShapeRenderer, PixmapLoader
from plugins import plugin_loadable
from network import SyncedVariables
from PyQt5.QtGui import QPainterPath, QPainter, QBrush, QColor, QPixmap, QTransform
from PyQt5.QtCore import Qt


@plugin_loadable
class PixmapShape(ShapeRenderer):
    def __init__(self, parent=None, data=None):
        super().__init__("pixmap", parent, data)

        self.cached = None
        self.prevColor = ""

    def tint(self, pixmap, color):
        tinted = QPixmap(pixmap.size())
        tinted.fill(Qt.transparent)

        painter = QPainter(tinted)
        painter.drawPixmap(0, 0, pixmap)

        painter.setCompositionMode(QPainter.CompositionMode_SourceIn)
        painter.fillRect(pixmap.rect(), color)

        painter.setCompositionMode(QPainter.CompositionMode_Multiply)
        painter.drawPixmap(0, 0, pixmap)

        painter.end()
        return tinted

    def draw(self, painter):
        pixmap = PixmapLoader.load_texture(self.data["path"])
        if pixmap is None:
            return

        if "color" in self.data and self.data["color"].upper() != "#FFFFFFFF" and self.data["color"].upper() != "#FFFFFF":
            if self.cached is None or self.prevColor != self.data["color"]:
                self.cached = self.tint(pixmap, QColor(self.data["color"]))
                self.prevColor = self.data["color"]
        elif self.cached is None:
            self.cached = pixmap
            
        
        justification = self.data["justification"] if "justification" in self.data else [0.5, 0.5]
        x, y, _, _ = self.parent.get_dimensions(self.data["x"], self.data["y"], 0, 0)

        sourceX = self.data["sourceX"] if "sourceX" in self.data else -1
        sourceY = self.data["sourceY"] if "sourceY" in self.data else -1
        sourceWidth = self.data["sourceWidth"] if "sourceWidth" in self.data else -1
        sourceHeight = self.data["sourceHeight"] if "sourceHeight" in self.data else -1
        width, height = sourceWidth if sourceWidth > -1 else self.cached.width(), sourceHeight if sourceHeight > -1 else self.cached.height()

        transform = painter.transform()
        painter.translate(x, y)
        if "rotation" in self.data:
            painter.rotate(self.data["rotation"])

        scaleX = self.data["scaleX"] if "scaleX" in self.data else 1
        scaleY = self.data["scaleY"] if "scaleY" in self.data else 1
        if scaleX != 1 or scaleY != 1:
            painter.scale(scaleX, scaleY)

        if sourceWidth <= 0 or sourceHeight <= 0:
            painter.drawPixmap(int(-width*justification[0]), int(-height*justification[1]), self.cached)
        else:
            sx = sourceX if sourceX >= 0 else width + sourceX
            sy = sourceY if sourceY >= 0 else height + sourceY
            painter.drawPixmap(int(-width*justification[0]), int(-height*justification[1]), self.cached, sx, sy, sourceWidth, sourceHeight)

        painter.setTransform(transform)