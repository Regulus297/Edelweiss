from ui import ShapeRenderer, PixmapLoader
from plugins import plugin_loadable
from network import SyncedVariables
from PyQt5.QtGui import QPainterPath, QPainter, QBrush, QColor
from PyQt5.QtCore import Qt


@plugin_loadable
class TileShape(ShapeRenderer):
    def __init__(self):
        super().__init__("tiles")

    def draw(self, painter, parent, data):
        pen = parent.get_pen(data)
        path = QPainterPath()
        path.addRect(0, 0, 16, 16)

        self.tiles = data["tileData"]
        self.width = data["width"]
        self.height = data["height"]

        painter.setPen(pen)
        tiles = SyncedVariables.variables["Edelweiss:ForegroundTiles"]
        keys = SyncedVariables.variables["Edelweiss:ForegroundTileKeys"]
        for i, tile in enumerate(self.tiles):
            if tile not in tiles:
                continue
            
            self.currentTile = tile
            x, y = i % data["width"], i // data["width"]
            tileX, tileY = self.pickTile(x, y, tiles[tile]).split(", ")
            tileX = int(tileX)
            tileY = int(tileY)
            painter.drawPixmap(x * 8, y * 8, PixmapLoader.load_texture(keys[tile]), tileX * 8, tileY * 8, 8, 8)
            

            


        painter.drawPath(path.simplified())
        painter.setRenderHint(QPainter.SmoothPixmapTransform, False)

    def pickTile(self, x, y, tileData):
        mask = self.generateMask(x, y, tileData)
        for mask_match, tiles in tileData.items():
            if self.maskMatches(mask, mask_match):
                index = self.pseudo_random(x - 9123, y - 40129, x + 932, y + 132547) % len(tiles)
                return tiles[index]
            
    @staticmethod
    def pseudo_random(a, b, c, d):
        seed = (a * 2654435761) ^ (b * 805459861) ^ (c * 1640531527) ^ (d * 337494731)
        seed ^= (seed >> 16)
        seed *= 0x85ebca6b
        seed ^= (seed >> 13)
        seed *= 0xc2b2ae35
        seed ^= (seed >> 16)
        return seed & 0xFFFFFFFF
            
    def generateMask(self, x, y, tileData):
        mask = ""
        for yOff in range(-1, 2):
            for xOff in range(-1, 2):
                xCur = x + xOff
                yCur = y + yOff
                mask += "1" if self.canConnectToCoords(xCur, yCur) else "0"
            mask += "-"

        if mask != "111-111-111-":
            return mask[:-1]

        if (self.canConnectToCoords(x-2, y) and
                self.canConnectToCoords(x, y-2) and
                self.canConnectToCoords(x+2, y) and
                self.canConnectToCoords(x, y+2)):
            return "center"
        return "padding"
    
    def canConnect(self, b):
        return b == self.currentTile or b == "✪" or b != " "

    def canConnectToCoords(self, x, y):
        return self.canConnect(self.getTile(x, y))

    def getTile(self, x, y):
        if x < 0 or x >= self.width or y < 0 or y >= self.height:
            return "✪"
        i = x + y * self.width
        return self.tiles[i]

    @staticmethod
    def maskMatches(a, b) -> bool:
        for charA, charB in zip(a, b):
            if charA == charB or charA == "x" or charB == "x":
               continue
            return False
        return True