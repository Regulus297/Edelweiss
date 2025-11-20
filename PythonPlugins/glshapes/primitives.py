from ui.opengl import GLShape
from OpenGL.GL import *
from PyQt5.QtGui import QColor
from plugins import plugin_loadable

import numpy as np

from ui.opengl.gl_shape import GLShape
from plugins import plugin_loadable

@plugin_loadable
class Rectangle(GLShape):
    def __init__(self, data=None, parent=None, name="rectangle"):
        if data is None:
            super().__init__(name)
            return
        
        self.x, self.y, self.w, self.h = data["x"], data["y"], data["width"], data["height"]
        self.r, self.g, self.b, self.a = 0, 0, 0, 0
        if "fill" in data:
            self.r, self.g, self.b, self.a = QColor(data["fill"]).getRgbF()

        self.rs, self.gs, self.bs, self._as = QColor(data["color"]).getRgbF()
        self.s = float(data["thickness"])
        self.parent = parent
        self.data = data

    def get_instance(self):
        if self.parent is None:
            return [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
        
        o = self.parent.draw_offset()
        return [self.x + o.x(), self.y + o.y(), self.w, self.h, self.r, self.g, self.b, self.a, self.rs, self.gs, self.bs, self._as, self.s]

    def vertex_count(self):
        return 6

    def initializer(self):
        verts = np.array([
            0, 0,
            1, 0,
            0, 1,
            1, 0,
            1, 1,
            0, 1
        ], dtype=np.float32)
        glBufferData(GL_ARRAY_BUFFER, verts.nbytes, verts, GL_STATIC_DRAW)

        glEnableVertexAttribArray(0)
        glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 8, ctypes.c_void_p(0))

    def instance_initializer(self):
        glEnableVertexAttribArray(1)
        glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, 52, ctypes.c_void_p(0))
        glVertexAttribDivisor(1, 1)
        
        glEnableVertexAttribArray(2)
        glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 52, ctypes.c_void_p(8))
        glVertexAttribDivisor(2, 1)

        glEnableVertexAttribArray(3)
        glVertexAttribPointer(3, 4, GL_FLOAT, GL_FALSE, 52, ctypes.c_void_p(16))
        glVertexAttribDivisor(3, 1)

        glEnableVertexAttribArray(4)
        glVertexAttribPointer(4, 4, GL_FLOAT, GL_FALSE, 52, ctypes.c_void_p(32))
        glVertexAttribDivisor(4, 1)

        glEnableVertexAttribArray(5)
        glVertexAttribPointer(5, 1, GL_FLOAT, GL_FALSE, 52, ctypes.c_void_p(48))
        glVertexAttribDivisor(5, 1)
        

    def vert(self):
        return """
        #version 330 core

        layout (location=0) in vec2 vertexPos;
        layout (location=1) in vec2 aPos;
        layout (location=2) in vec2 aSize;
        layout (location=3) in vec4 aColor;
        layout (location=4) in vec4 aStrokeColor;
        layout (location=5) in float aStrokeThickness;

        uniform mat4 uCamera;

        out vec4 fragmentColor;
        out vec4 fragmentStrokeColor;
        out vec2 fragUV;
        out vec2 fragThickness;

        void main() {
            fragUV = vertexPos;
            vec2 world = aPos + vertexPos * aSize;
            gl_Position = uCamera * vec4(world, 0.0, 1.0);
            fragmentColor = aColor;
            fragmentStrokeColor = aStrokeColor;
            fragThickness = vec2(aStrokeThickness) / aSize;
        }
        """

    def frag(self):
        return """
        #version 330 core

        in vec4 fragmentColor;
        in vec4 fragmentStrokeColor;
        in vec2 fragUV;
        in vec2 fragThickness;

        out vec4 color;


        void main() {
            if(fragUV.x < fragThickness.x || fragUV.y < fragThickness.y || 1 - fragUV.x < fragThickness.x || 1 - fragUV.y < fragThickness.y) {
                color = fragmentStrokeColor;
                return;
            }
            color = fragmentColor;
        }
        """

    def instanced(self):
        return True


@plugin_loadable
class Circle(Rectangle):
    def __init__(self, data=None, parent=None):
        if data is None:
            super().__init__(name="circle")
            return

        x, y, rad = data["x"], data["y"], data["radius"]
        new_data = {
            "x": x - rad,
            "y": y - rad,
            "width": 2 * rad,
            "height": 2 * rad,
            "color": data["color"],
            "thickness": data["thickness"]
        }
        if "fill" in data:
            new_data["fill"] = data["fill"]

        self.parent = parent
        self.data = data
        super().__init__(new_data, parent)

    def frag(self):
        return """
        #version 330 core

        in vec4 fragmentColor;
        in vec4 fragmentStrokeColor; 
        in vec2 fragUV;
        in vec2 fragThickness;

        out vec4 color;


        void main() {
            vec2 cUV = fragUV + vec2(-0.5, -0.5);
            float dist = length(cUV);
            if(dist > 0.5) {
                color = vec4(0, 0, 0, 0);
                return;
            }
            if(0.5 - dist < fragThickness.x) {
                color = fragmentStrokeColor;
                return;
            }
            color = fragmentColor;
        }
        """

@plugin_loadable
class Line(GLShape):
    def __init__(self, data=None, parent=None):
        if data is None:
            super().__init__("line")
            return

        self.x1, self.y1, self.x2, self.y2 = data["x1"], data["y1"], data["x2"], data["y2"]
        self.r, self.g, self.b, self.a = QColor(data["color"]).getRgbF()
        self.thickness = data["thickness"]
        self.parent = parent

    def get_instance(self):
        if self.parent is None:
            return [0, 0, 0, 0, 0, 0, 0, 0, 0]

        o = self.parent.draw_offset()
        return [self.x1 + o.x(), self.y1 + o.y(), self.x2 + o.x(), self.y2 + o.y(), self.r, self.g, self.b, self.a, self.thickness]

    def initializer(self):
        verts = np.array([
            -1, -1,
             1, -1,
            -1,  1,
             1, -1,
             1,  1,
            -1,  1
        ], dtype=np.float32)
        glBufferData(GL_ARRAY_BUFFER, verts.nbytes, verts, GL_STATIC_DRAW)

        glEnableVertexAttribArray(0)
        glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 8, ctypes.c_void_p(0))

    def instance_initializer(self):
        stride = 36

        glEnableVertexAttribArray(1)
        glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(0))
        glVertexAttribDivisor(1, 1)

        glEnableVertexAttribArray(2)
        glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(8))
        glVertexAttribDivisor(2, 1)

        glEnableVertexAttribArray(3)
        glVertexAttribPointer(3, 4, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(16))
        glVertexAttribDivisor(3, 1)

        glEnableVertexAttribArray(4)
        glVertexAttribPointer(4, 1, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(32))
        glVertexAttribDivisor(4, 1)

    def instanced(self):
        return True

    def vertex_count(self):
        return 6

    def vert(self):
        return """
        #version 330 core
        
        layout (location=0) in vec2 vPos;
        
        layout (location=1) in vec2 aPos1;
        layout (location=2) in vec2 aPos2;
        layout (location=3) in vec4 aColor;
        layout (location=4) in float aThickness;
        
        uniform mat4 uCamera;
        
        out vec4 fragColor;
        
        void main() {
            vec2 dir = normalize(aPos2 - aPos1);
            vec2 perp = vec2(-dir.y, dir.x);
            
            float t = vPos.x * 0.5 + 0.5;
            vec2 pos = mix(aPos1, aPos2, t);
            
            pos += perp * vPos.y * aThickness * 0.5;
            gl_Position = uCamera * vec4(pos, 0, 1);
            fragColor = aColor;
        }
        """

    def frag(self):
        return """
        #version 330 core
        
        in vec4 fragColor;
        
        out vec4 color;
        
        void main() {
            color = fragColor;
        }
        """