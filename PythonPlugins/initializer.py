from ui import PixmapLoader
from network import SyncedVariables
from plugins import plugin_loadable

@plugin_loadable
class Initializer:
    def __init__(self):
        SyncedVariables.add_listener("Edelweiss:Textures", PixmapLoader.clear_cache)