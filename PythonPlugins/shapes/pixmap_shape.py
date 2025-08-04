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
        painter.drawPixmap(int(x - width*justification[0]), int(y - height*justification[1]), pixmap)
