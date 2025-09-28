from ui import ShapeRenderer, PixmapLoader
from plugins import plugin_loadable, load_dependencies
from network import SyncedVariables
from PyQt5.QtGui import QPainterPath, QPainter, QBrush, QColor, QPixmap
from PyQt5.QtCore import Qt


@load_dependencies("pixmap_shape.py")
@plugin_loadable
class NinePatchShape(ShapeRenderer):
    def __init__(self, parent=None, data=None):
        super().__init__("ninePatch", parent, data)
        self._cache = None

    def draw(self, painter):
        if self._cache is None:
            self._redraw_cache()
        if self._cache is None:
            return
        painter.drawPixmap(self.data["x"], self.data["y"], self._cache)

    def _redraw_cache(self):
        parent = self.parent
        data = self.data

        x = self.data["x"]
        y = self.data["y"]
        w = self.data["width"]
        h = self.data["height"]
        
        bl = self.data["borderLeft"]
        br = self.data["borderRight"]
        bt = self.data["borderTop"]
        bb = self.data["borderBottom"]

        tw = self.data["tileWidth"]
        th = self.data["tileHeight"]

        mode = self.data["mode"]
        bm = self.data["borderMode"]
        fm = self.data["fillMode"]

        td = self.data["textureData"]

        ax = td["atlasX"]
        ay = td["atlasY"]
        aw = td["atlasWidth"]
        ah = td["atlasHeight"]

        texw = td["width"]
        texh = td["height"]


        self._cache = QPixmap(w, h)
        self._cache.fill(QColor(0, 0, 0, 0))

        painter = QPainter(self._cache)
        pixmap = PixmapLoader.load_texture("Gameplay/" + data["texture"])

        
        if "color" in data and data["color"].upper() != "#FFFFFF" and data["color"].upper != "#FFFFFFFF":
            pixmap = PixmapLoader.tint(pixmap, QColor(data["color"]), "Gameplay/" + data["texture"], data["color"])


        if pixmap is None:
            self._cache = None
            return

        # Corners
        if w > 0 and h > 0 and bl > 0 and bt > 0:
            painter.drawPixmap(0, 0, pixmap, ax, ay, bl, bt)

        if w > 0 and h > 0 and br > 0 and bt > 0:
            painter.drawPixmap(w - br, 0, pixmap, ax + texw - br, ay, br, bt)
        
        if w > 0 and h > 0 and bl > 0 and bb > 0:
            painter.drawPixmap(0, h - bb, pixmap, ax, ay + texh - bb, bl, bb)
        
        if w > 0 and h > 0 and br > 0 and bb > 0:
            painter.drawPixmap(w - br, h - bb, pixmap, ax + texw - br, ay + texh - bb, br, bb)

        # Horizontal edges
        for cx in range(bl, w - br, tw):
            sourceX = (cx - bl) % (texw - (bl + br))
            painter.drawPixmap(cx, 0, pixmap, ax + sourceX + bl, ay, tw, bt)
            painter.drawPixmap(cx, h - bb, pixmap, ax + sourceX + bl, ay + texh - bb, tw, bb)

        # Vertical edges
        for cy in range(bt, h - bl, th):
            sourceY = (cy - bt) % (texh - (bt + bb))
            painter.drawPixmap(0, cy, pixmap, ax, ay + sourceY + bt, bl, th)
            painter.drawPixmap(w - br, cy, pixmap, ax + texw - br, ay + sourceY + bt, br, th)

        
        if mode == "fill":
            for cx in range(bl, w - br, tw):
                for cy in range(bt, h - bl, th):
                    sourceX = (cx - bl) % (texw - (bl + br))
                    sourceY = (cy - bt) % (texh - (bt + bb))
                    painter.drawPixmap(cx, cy, pixmap, ax + sourceX + bl, ay + sourceY + bt, tw, th)
