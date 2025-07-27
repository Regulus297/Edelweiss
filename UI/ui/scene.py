import json

from PyQt5.QtWidgets import QGraphicsItem, QGraphicsPixmapItem

from ui.custom_draw_item import CustomDrawItem
from ui.mapping_scene import MappingScene


class Scene:
    def __init__(self):
        self.elements = {}

        self.scene_width = 64000
        self.scene_height = 64000
        self.grScene: MappingScene = MappingScene(self)

        self.initUI()

    def initUI(self):
        self.grScene.setScene(self.scene_width, self.scene_height)

    def addItem(self, data):
        item = CustomDrawItem(json.loads(data))
        self.grScene.addItem(item)
        self.elements[item.name] = item

    def addShape(self, data):
        data = json.loads(data)
        self.elements[data["name"]].shapes.append(data["shape"])