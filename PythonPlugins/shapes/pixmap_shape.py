from ui import ShapeRenderer, PixmapLoader
from plugins import plugin_loadable
from network import SyncedVariables
from PyQt5.QtGui import QPainterPath, QPainter, QBrush, QColor, QPixmap
from PyQt5.QtCore import Qt


@plugin_loadable
class PixmapShape(ShapeRenderer):
    def __init__(self, parent=None, data=None):
        super().__init__("pixmap", parent, data)

    def draw(self, painter):
        pixmap = PixmapLoader.load_texture(self.data["path"])
        if pixmap is None:
            return
        
        painter.setOpacity(1 if "opacity" not in self.data else float(self.data["opacity"]))
        
        justification = self.data["justification"] if "justification" in self.data else [0.5, 0.5]
        x, y, _, _ = self.parent.get_dimensions(self.data["x"], self.data["y"], 0, 0)
        width, height = pixmap.width(), pixmap.height()

        sourceX = self.data["sourceX"] if "sourceX" in self.data else -1
        sourceY = self.data["sourceY"] if "sourceY" in self.data else -1
        sourceWidth = self.data["sourceWidth"] if "sourceWidth" in self.data else -1
        sourceHeight = self.data["sourceHeight"] if "sourceHeight" in self.data else -1

        if sourceWidth <= 0 or sourceHeight <= 0:
            painter.drawPixmap(int(x - width*justification[0]), int(y - height*justification[1]), pixmap)
        else:
            sx = sourceX if sourceX >= 0 else width + sourceX
            sy = sourceY if sourceY >= 0 else height + sourceY
            painter.drawPixmap(int(x - width*justification[0]), int(y - height*justification[1]), pixmap, sx, sy, sourceWidth, sourceHeight)
