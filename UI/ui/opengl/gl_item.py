from PyQt5.QtCore import QPointF

from .gl_widget import GLWidget
from .gl_shape import GLShape


class GLItem:
    def __init__(self, data, parent=None):
        self.x = data["x"]
        self.y = data["y"]
        self.name = data["name"]
        self.parent: GLItem = parent

        self.widget: GLWidget = None

        self.shapes = [GLShape.create(shape["type"], shape, self) for shape in data["shapes"]]

    def draw_offset(self):
        if self.parent is None:
            return QPointF(self.x, self.y)
        return self.parent.draw_offset() + QPointF(self.x, self.y)

    @property
    def tracker(self):
        if self.parent is None:
            return self.name
        return f"{self.parent.tracker}/{self.name}"

    def refresh(self, data):
        if "x" in data:
            self.x = data["x"]
        if "y" in data:
            self.y = data["y"]
        if "name" in data:
            self.name = data["name"]

        if "shapes" in data:
            self.clear()
            for shape in data["shapes"]:
                self.addShape(shape)
        else:
            for shape in self.shapes:
                arr = self.widget.getVertexArray(shape)
                if arr is not None:
                    arr.prepare_refresh()
        self.widget.update()

    def addShape(self, shape):
        self.shapes.append(GLShape.create(shape["type"], shape, self))
        arr = self.widget.getVertexArray(self.shapes[-1])
        if arr is not None:
            arr.add_shape(self.shapes[-1])
        self.widget.update()

    def clear(self):
        for shape in self.shapes:
            arr = self.widget.getVertexArray(shape)
            if arr is not None:
                arr.remove_shape(shape)
        self.shapes.clear()
        self.widget.update()

    def delete(self):
        self.clear()
        del self.widget.items[self.tracker]
        self.widget.update()
