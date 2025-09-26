import zipfile

from PyQt5.QtGui import QPixmap

from network import SyncedVariables


class PixmapLoader:
    loaded_textures = {}
    loaded_paths = {}

    loaded_tints = {}

    @staticmethod
    def load_texture(key):
        if key in PixmapLoader.loaded_textures:
            return PixmapLoader.loaded_textures[key]

        textures = SyncedVariables.variables["Edelweiss:Textures"]
        if key in textures:
            path = textures[key]
            if path in PixmapLoader.loaded_paths:
                return PixmapLoader.loaded_paths[path]
            if chr(0) in path:
                zip_path, name = path.split(chr(0))
                with zipfile.ZipFile(zip_path) as mod:
                    pixmap = QPixmap()
                    pixmap.loadFromData(mod.read(name))
                    if pixmap.width() == 0 or pixmap.height() == 0:
                        return None
                    PixmapLoader.loaded_textures[key] = PixmapLoader.loaded_paths[path] = pixmap
                    return pixmap

            PixmapLoader.loaded_textures[key] = PixmapLoader.loaded_paths[path] = QPixmap(path)
            return PixmapLoader.loaded_textures[key]
        return None

    @staticmethod
    def clear_cache():
        PixmapLoader.loaded_textures = {}
        PixmapLoader.loaded_paths = {}