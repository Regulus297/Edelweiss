from PyQt5.QtGui import QPixmap

from network import SyncedVariables


class PixmapLoader:
    loaded_textures = {}

    @staticmethod
    def load_texture(key):
        if key in PixmapLoader.loaded_textures:
            return PixmapLoader.loaded_textures[key]

        textures = SyncedVariables.variables["Edelweiss:Textures"]
        if key in textures:
            PixmapLoader.loaded_textures[key] = QPixmap(textures[key])
            return PixmapLoader.loaded_textures[key]
        return None
