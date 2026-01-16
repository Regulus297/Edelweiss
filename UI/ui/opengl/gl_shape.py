from OpenGL.GL import *
from OpenGL.GL.shaders import *


class GLShape:
    shapes = {}

    def __init__(self, name):
        GLShape.shapes[name] = [self.vert(), self.frag(), self.initializer, self.instance_initializer, self.extras_initializer, self.vertex_count(), self.instanced(), type(self)]
        self.parent = None
        self.data = {}


    @staticmethod
    def create(name, data, parent):
        if name not in GLShape.shapes:
            return None
        return GLShape.shapes[name][-1](data, parent)

    def vert(self):
        return ""

    def frag(self):
        return ""

    def vertex_count(self):
        return 0

    def initializer(self):
        ...

    def vertex_location_count(self):
        return 0

    def instance_initializer(self):
        c = self.vertex_location_count()
        glEnableVertexAttribArray(c)
        glVertexAttribPointer(c, 1, GL_FLOAT, GL_FALSE, 4, ctypes.c_void_p(0))
        glVertexAttribDivisor(c, 1)

    def extras_initializer(self, arr):
        ...

    def get_vertices(self):
        return []

    def get_instance(self):
        return [0]

    @staticmethod
    def compileShader(vertSrc, fragSrc):
        return compileProgram(
            compileShader(vertSrc, GL_VERTEX_SHADER),
            compileShader(fragSrc, GL_FRAGMENT_SHADER)
        )

    def instanced(self):
        return False

    def refresh(self, data):
        self.data.update(data)
        self.__init__(self.parent, self.data)