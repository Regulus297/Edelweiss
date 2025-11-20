import random

import numpy as np
from PyQt5.QtCore import QPoint, QPointF, Qt
from PyQt5.QtGui import QColor
from PyQt5.QtWidgets import QOpenGLWidget
from OpenGL.GL import *
from OpenGL.GL.shaders import compileProgram, compileShader

from .vertex_array import VertexArray
from .gl_shape import GLShape


class GLWidget(QOpenGLWidget):
    def __init__(self, parent):
        super().__init__(parent)

        self.pan = np.array([1.0, 0.0], dtype=np.float32)
        self.zoom = 1
        self.items = {}
        self.vertexArrays = {}
        self.setFocusPolicy(Qt.StrongFocus)

        self.onMouseMoved = lambda x, y : None
        self.setMouseTracking(True)

    def initializeGL(self):
        glEnable(GL_BLEND)
        glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA)
        for shape in GLShape.shapes.keys():
            self.vertexArrays[GLShape.shapes[shape][-1]] = VertexArray(shape)

    def resizeGL(self, w, h):
        glViewport(0, 0, w, h)

    def paintGL(self):
        glClearColor(0.12, 0.12, 0.12, 1.0)
        glClear(GL_COLOR_BUFFER_BIT)

        for arr in self.vertexArrays.values():
            glUseProgram(arr.shader)
            arr.refresh()
            loc = glGetUniformLocation(arr.shader, "uCamera")
            glUniformMatrix4fv(loc, 1, GL_FALSE, self._get_cam_matrix())
            glBindVertexArray(arr.vao)
            glDrawArraysInstanced(GL_TRIANGLES, 0, arr.vertex_count, arr.instance_count)

    def _get_cam_matrix(self):
        w = max(1, self.width())
        h = max(1, self.height())

        sx = self.zoom * 2.0 / w
        sy = -self.zoom * 2.0 / h
        tx = -sx * self.pan[0]
        ty =  -sy * self.pan[1]

        mat = np.array([
            [sx,  0, 0, tx],
            [0,  sy, 0, ty],
            [0,   0, 1,  0],
            [0,   0, 0,  1]
        ], dtype=np.float32)

        return mat.T

    def wheelEvent(self, event):
        zoom_factor = 1.0 + event.angleDelta().y() / 1200.0
        pos = self.screen_to_world(event.pos())
        self.zoom *= zoom_factor

        new_pos = self.screen_to_world(event.pos())

        self.pan[0] -= new_pos.x() - pos.x()
        self.pan[1] -= new_pos.y() - pos.y()

        self.update()

    def screen_to_world(self, point: QPointF):
        offset = QPointF(-self.width()/2, -self.height()/2)
        offset += point
        offset /= self.zoom
        offset += QPointF(self.pan[0], self.pan[1])
        return offset

    def addItem(self, item):
        self.items[item.tracker] = item
        item.widget = self
        for shape in item.shapes:
            if shape is None:
                continue
            self.getVertexArray(shape).add_shape(shape)

    @property
    def pen_size(self):
        s = 4/self.zoom
        return max(1.0, min(s, 20.0))

    def getVertexArray(self, shape):
        if shape is None:
            return None
        return self.vertexArrays[type(shape)]

    def clear(self):
        for item in self.items.values():
            item.clear()
        self.items.clear()

    def mouseMoveEvent(self, a0):
        if self.onMouseMoved:
            world = self.screen_to_world(a0.pos())
            self.onMouseMoved(world.x(), world.y())