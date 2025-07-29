from ui import CustomDrawItem


class ShapeRenderer:
    def __init__(self, shape_type):
        CustomDrawItem.shapes[shape_type] = self

    def draw(self, painter, parent, data):
        ...