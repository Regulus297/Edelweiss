import zipfile

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
            path = textures[key]
            if chr(0) in path:
                zip_path, name = path.split(chr(0))
                with zipfile.ZipFile(zip_path) as mod:
                    pixmap = QPixmap()
                    pixmap.loadFromData(mod.read(name))
                    PixmapLoader.loaded_textures[key] = pixmap
                    return pixmap

            PixmapLoader.loaded_textures[key] = QPixmap(path)
            return PixmapLoader.loaded_textures[key]
        return None
