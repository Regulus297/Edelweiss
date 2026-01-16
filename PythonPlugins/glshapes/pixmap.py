import ctypes

from OpenGL.GL import *
from OpenGL.raw.GL.ARB.bindless_texture import *
from PyQt5.QtGui import QColor
import numpy as np

from ui.opengl.gl_shape import GLShape
from ui.pixmap_loader import PixmapLoader
from plugins import plugin_loadable


@plugin_loadable
class Pixmap(GLShape):
    bindless_locs = {}

    def __init__(self, data=None, parent=None):
        if data is None:
            super().__init__("pixmap")
            return

        self.parent = parent
        self.x, self.y = data["x"], data["y"]
        self.jx, self.jy = data["justification"]

        self.sx = data["sourceX"] if "sourceX" in data else -1
        self.sy = data["sourceY"] if "sourceY" in data else -1
        self.sw = data["sourceWidth"] if "sourceWidth" in data else -1
        self.sh = data["sourceHeight"] if "sourceHeight" in data else -1

        self.r, self.g, self.b, self.a = 1, 1, 1, 1
        if "color" in data:
            self.r, self.g, self.b, self.a = QColor(data["color"]).getRgbF()

        self.rot = data["rotation"] if "rotation" in data else 0

        self.scx = data["scaleX"] if "scaleX" in data else 1
        self.scy = data["scaleY"] if "scaleY" in data else 1

        self.img, w, h = PixmapLoader.load_bindless_texture(data["path"])
        self.key = data["path"]

        self.w = self.sw if self.sw > -1 else w
        self.h = self.sh if self.sh > -1 else h

        self.ox = data["offsetX"] if "offsetX" in data else 0
        self.oy = data["offsetY"] if "offsetY" in data else 0

        self.pw = data["paddedWidth"] if "paddedWidth" in data else self.w
        self.ph = data["paddedHeight"] if "paddedHeight" in data else self.h

        self.depth = float(data["depth"])

        self.imgW = w
        self.imgH = h

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

    def get_instance(self):
        if self.parent is None:
            return [0 for _ in range(23)]

        o = self.parent.draw_offset()
        return [self.x + o.x(), self.y + o.y(), self.jx, self.jy, self.sx, self.sy, self.sw, self.sh, self.imgW,
                self.imgH, self.w, self.h, self.scx, self.scy, self.rot, self.ox, self.oy, self.pw, self.ph, self.r,
                self.g, self.b, self.a]

    def instance_initializer(self):
        stride = 23 * 4

        glEnableVertexAttribArray(1)
        glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(0))
        glVertexAttribDivisor(1, 1)

        glEnableVertexAttribArray(2)
        glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(8))
        glVertexAttribDivisor(2, 1)

        glEnableVertexAttribArray(3)
        glVertexAttribPointer(3, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(16))
        glVertexAttribDivisor(3, 1)

        glEnableVertexAttribArray(4)
        glVertexAttribPointer(4, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(24))
        glVertexAttribDivisor(4, 1)

        glEnableVertexAttribArray(5)
        glVertexAttribPointer(5, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(32))
        glVertexAttribDivisor(5, 1)

        glEnableVertexAttribArray(6)
        glVertexAttribPointer(6, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(40))
        glVertexAttribDivisor(6, 1)

        glEnableVertexAttribArray(7)
        glVertexAttribPointer(7, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(48))
        glVertexAttribDivisor(7, 1)

        glEnableVertexAttribArray(8)
        glVertexAttribPointer(8, 1, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(56))
        glVertexAttribDivisor(8, 1)

        glEnableVertexAttribArray(9)
        glVertexAttribPointer(9, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(60))
        glVertexAttribDivisor(9, 1)

        glEnableVertexAttribArray(10)
        glVertexAttribPointer(10, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(68))
        glVertexAttribDivisor(10, 1)

        glEnableVertexAttribArray(11)
        glVertexAttribPointer(11, 4, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(76))
        glVertexAttribDivisor(11, 1)

    def extras_initializer(self, arr):
        handles = []
        for shape in arr.shapes:
            loc = shape.texture_loc
            lower =  loc & 0xFFFFFFFF
            upper = loc >> 32
            handles.append(lower)
            handles.append(upper)

        handles = np.array(handles, dtype=np.uint32)

        arr.texture_vbo = glGenBuffers(1)
        glBindBuffer(GL_ARRAY_BUFFER, arr.texture_vbo)
        glBufferData(GL_ARRAY_BUFFER, handles.nbytes, handles, GL_STATIC_DRAW)

        glEnableVertexAttribArray(12)
        glVertexAttribIPointer(12, 2, GL_UNSIGNED_INT, 8, ctypes.c_void_p(0))
        glVertexAttribDivisor(12, 1)

    @staticmethod
    def upload_texture(key, data, width, height):
        if data is None:
            return 0

        if key in Pixmap.bindless_locs:
            return Pixmap.bindless_locs[key]

        tex = glGenTextures(1)
        glBindTexture(GL_TEXTURE_2D, tex)

        glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA8, width, height, 0, GL_RGBA, GL_UNSIGNED_BYTE, data)
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST)
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST)
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE)
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE)
        h = glGetTextureHandleARB(tex)
        glMakeTextureHandleResidentARB(h)
        Pixmap.bindless_locs[key] = h
        return h

    @property
    def texture_loc(self):
        return Pixmap.upload_texture(self.key, self.img, self.imgW, self.imgH)

    def vert(self):
        return """
        #version 460 core

        layout (location=0) in vec2 vPos;

        layout (location=1) in vec2 aPos;
        layout (location=2) in vec2 aJust;
        layout (location=3) in vec2 aSourcePos;
        layout (location=4) in vec2 aSourceSize;
        layout (location=5) in vec2 aImageRes;
        layout (location=6) in vec2 aSize;
        layout (location=7) in vec2 aScale;
        layout (location=8) in float aRotation;
        layout (location=9) in vec2 aOffset;
        layout (location=10) in vec2 aPaddedSize;
        layout (location=11) in vec4 aTint;

        layout (location=12) in uvec2 aHandle;

        flat out uvec2 fHandle;
        out vec2 fUV;
        out vec4 fTint;

        uniform mat4 uCamera;

        void main() {
            fHandle = aHandle;
            fTint = aTint;

            vec2 pos = aPos + vPos * aSize;
            vec2 anchor = aPos + aJust * aSize;
            pos -= anchor;

            mat2 scale = mat2(aScale.x, 0, 0, aScale.y);
            float angle = aRotation * 3.1415 / 180;
            mat2 rot = mat2(cos(angle), -sin(angle), sin(angle), cos(angle));
            pos = rot * scale * pos; 

            pos += anchor;
            pos += aOffset;
            pos -= aPaddedSize * aJust;
            gl_Position = uCamera * vec4(pos, 0, 1);      

            if(aSourcePos.x >= 0 && aSourcePos.y >= 0 && aSourceSize.x >= 0 && aSourceSize.y >= 0) {
                vec2 sourceUV = aSourcePos / aImageRes;
                vec2 sourceUVSize = aSourceSize / aImageRes;
                fUV = sourceUV + vPos * sourceUVSize;
            }
            else {
                fUV = vPos;
            }
        } 
        """

    def frag(self):
        return """
        #version 460 core
        #extension GL_ARB_bindless_texture : require

        in flat uvec2 fHandle;
        in vec2 fUV;
        in vec4 fTint;

        out vec4 fragColor;

        void main() {
            if(fHandle.x == 0 && fHandle.y == 0) {
                fragColor = vec4(0, 0, 0, 0);
                return;
            }
            sampler2D tex = sampler2D(fHandle);
            fragColor = texture(tex, fUV) * fTint;
        } 
        """

    def vertex_count(self):
        return 6

    def instanced(self):
        return True
