from .shape_item import ShapeItem


class ShapeRenderer:
    def __init__(self, shape_type, parent, data):
        ShapeItem.shapes[shape_type] = self
        self.parent = parent
        self.data = data

    def draw(self, painter):
        ...