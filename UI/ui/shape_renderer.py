from ui import CustomDrawItem


class ShapeRenderer:
    def __init__(self, shape_type, parent, data):
        CustomDrawItem.shapes[shape_type] = self
        self.parent = parent
        self.data = data

    def draw(self, painter):
        ...