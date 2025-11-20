from OpenGL.GL import *
import numpy as np
from .gl_shape import GLShape


class VertexArray:
    def __init__(self, shape_type):
        self.data = GLShape.shapes[shape_type]
        self.shader = GLShape.compileShader(self.data[0], self.data[1])
        self.initializer = self.data[2]
        self.instance_initializer = self.data[3]
        self.instanced = self.data[5]
        self.vao = glGenVertexArrays(1)
        self.vbo = glGenBuffers(1)
        self.instance_vbo = glGenBuffers(1)
        self.shapes = []

        self._dirty = False

        self.upload_shaders()

    def refresh(self):
        if self._dirty:
            self.upload_shaders()
            self._dirty = False

    def add_shape(self, shape):
        self.shapes.append(shape)
        self._dirty = True

    def remove_shape(self, shape):
        self.shapes.remove(shape)
        self._dirty = True

    def prepare_refresh(self):
        self._dirty = True

    def upload_shaders(self):
        glBindVertexArray(self.vao)

        data = []
        instance_data = []
        for shape in self.shapes:
            data.extend(shape.get_vertices())
            instance_data.extend(shape.get_instance())

        data = np.array(data, dtype=np.float32)

        instance_data = np.array(instance_data, dtype=np.float32)

        glBindBuffer(GL_ARRAY_BUFFER, self.vbo)
        glBufferData(GL_ARRAY_BUFFER, data.nbytes, data, GL_STATIC_DRAW)
        self.initializer()

        glBindVertexArray(self.vao)
        glBindBuffer(GL_ARRAY_BUFFER, self.instance_vbo)
        glBufferData(GL_ARRAY_BUFFER, instance_data.nbytes, instance_data, GL_STATIC_DRAW)
        self.instance_initializer()

    @property
    def vertex_count(self):
        if self.instanced:
            return self.data[4]
        return self.data[4] * len(self.shapes)

    @property
    def instance_count(self):
        if self.instanced:
            return len(self.shapes)
        return 1
