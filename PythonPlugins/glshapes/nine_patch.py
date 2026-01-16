import numpy as np
from OpenGL.GL import *
from PyQt5.QtGui import QColor

from ui.opengl import GLShape
from ui import PixmapLoader
from plugins import load_dependencies, plugin_loadable


@load_dependencies("pixmap.py")
@plugin_loadable
class NinePatch(GLShape):
    def __init__(self, data=None, parent=None):
        if data is None:
            super().__init__("ninePatch")
            return

        self.x = data["x"]
        self.y = data["y"]
        self.w = data["width"]
        self.h = data["height"]

        self.bl = data["borderLeft"]
        self.br = data["borderRight"]
        self.bt = data["borderTop"]
        self.bb = data["borderBottom"]

        self.tw = data["tileWidth"]
        self.th = data["tileHeight"]

        self.mode = 0 if data["mode"] == "fill" else 1
        self.bm = 0 if data["borderMode"] == "random" else 1
        self.fm = 0 if data["fillMode"] == "random" else 1

        self.td = data["textureData"]

        self.ax = self.td["atlasX"]
        self.ay = self.td["atlasY"]
        self.aw = self.td["atlasWidth"]
        self.ah = self.td["atlasHeight"]

        self.texw = self.td["width"]
        self.texh = self.td["height"]

        self.r, self.g, self.b, self.a = 1, 1, 1, 1
        if "color" in data:
            self.r, self.g, self.b, self.a = QColor(data["color"]).getRgbF()

        self.key = "Gameplay/" + data["texture"]
        self.img, self.imgW, self.imgH = PixmapLoader.load_bindless_texture(self.key)
        self.parent = parent

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
        if self.texture_loc is None:
            return []

        if self.parent is None:
            return [0 for _ in range(25)]

        o = self.parent.draw_offset()
        return [self.x + o.x(), self.y + o.y(), self.w, self.h, self.bl, self.br, self.bt, self.bb, self.tw, self.th,
                self.mode, self.bm, self.fm, self.ax, self.ay, self.aw, self.ah, self.texw, self.texh, self.r, self.g,
                self.b, self.a, self.imgW, self.imgH]

    def instance_initializer(self):
        stride = 25 * 4

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
        glVertexAttribPointer(4, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(32))
        glVertexAttribDivisor(4, 1)

        glEnableVertexAttribArray(5)
        glVertexAttribPointer(5, 3, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(40))
        glVertexAttribDivisor(5, 1)

        glEnableVertexAttribArray(6)
        glVertexAttribPointer(6, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(52))
        glVertexAttribDivisor(6, 1)

        glEnableVertexAttribArray(7)
        glVertexAttribPointer(7, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(60))
        glVertexAttribDivisor(7, 1)

        glEnableVertexAttribArray(8)
        glVertexAttribPointer(8, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(68))
        glVertexAttribDivisor(8, 1)

        glEnableVertexAttribArray(9)
        glVertexAttribPointer(9, 4, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(76))
        glVertexAttribDivisor(9, 1)

        glEnableVertexAttribArray(10)
        glVertexAttribPointer(10, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(92))
        glVertexAttribDivisor(10, 1)

    def extras_initializer(self, arr):
        handles = []
        for shape in arr.shapes:
            loc = shape.texture_loc
            lower = loc & 0xFFFFFFFF
            upper = loc >> 32
            handles.append(lower)
            handles.append(upper)

        handles = np.array(handles, dtype=np.uint32)

        arr.texture_vbo = glGenBuffers(1)
        glBindBuffer(GL_ARRAY_BUFFER, arr.texture_vbo)
        glBufferData(GL_ARRAY_BUFFER, handles.nbytes, handles, GL_STATIC_DRAW)

        glEnableVertexAttribArray(11)
        glVertexAttribIPointer(11, 2, GL_UNSIGNED_INT, 8, ctypes.c_void_p(0))
        glVertexAttribDivisor(11, 1)

    @property
    def texture_loc(self):
        return Pixmap.upload_texture(self.key, self.img, self.imgW, self.imgH)

    def vert(self):
        return """
        #version 460 core
        
        layout (location=0) in vec2 vPos;
        
        layout (location=1) in vec2 aPos;
        layout (location=2) in vec2 aSize;
        layout (location=3) in vec4 aBorder;
        layout (location=4) in vec2 aTileSize;
        layout (location=5) in vec3 aModes;
        layout (location=6) in vec2 aSourcePos;
        layout (location=7) in vec2 aSourceSize;
        layout (location=8) in vec2 aTextureSize;
        layout (location=9) in vec4 aTint;
        layout (location=10) in vec2 aImageRes;
        
        layout (location=11) in uvec2 aHandle;
        
        out vec2 fUV;
        out vec2 fSize;
        out vec4 fTint;
        out vec4 fBorder;
        out vec2 fTileSize;
        out vec2 fSourcePos;
        out vec2 fSourceSize;
        out vec2 fTextureSize;
        flat out uvec2 fHandle;

        uniform mat4 uCamera;
        
        void main() {
            fTint = aTint;
            fHandle = aHandle;
            vec2 pos = aPos + vPos * aSize;
            fUV = vPos * aSize;
            fSize = aSize;
            fBorder = aBorder;
            fTileSize = aTileSize;
            fSourcePos = aSourcePos;
            fSourceSize = aSourceSize;
            fTextureSize = aImageRes;
            gl_Position = uCamera * vec4(pos, 0.0, 1.0);
        }
        """

    def frag(self):
        return """
        #version 460 core
        #extension GL_ARB_bindless_texture : require
        
        in vec2 fUV;
        in vec2 fSize;
        in vec4 fTint;
        in vec4 fBorder;
        in vec2 fTileSize;
        in vec2 fSourcePos;
        in vec2 fSourceSize;
        in vec2 fTextureSize;
        flat in uvec2 fHandle;
        
        out vec4 color;
        
        void main() {
        
            if(fHandle.x == 0 && fHandle.y == 0) {
                color = vec4(0, 0, 0, 0);
                return;
            }

            sampler2D tex = sampler2D(fHandle);
            if(fUV.x < fBorder.x && fUV.y < fBorder.z) {
                color = texture(tex, (fSourcePos + fUV)/fTextureSize);
            }
            else if(fUV.x < fBorder.x && (fSize.y - fUV.y) < fBorder.w) {
                color = texture(tex, vec2(fSourcePos.x + fUV.x, fSourcePos.y + fSourceSize.y + fUV.y - fSize.y)/fTextureSize);
            }
            else if((fSize.x - fUV.x) < fBorder.y && fUV.y < fBorder.z) {
                color = texture(tex, vec2(fSourcePos.x + fSourceSize.x + fUV.x - fSize.x, fSourcePos.y + fUV.y)/fTextureSize);
            }
            else if((fSize.x - fUV.x) < fBorder.y && (fSize.y - fUV.y) < fBorder.w) {
                color = texture(tex, vec2(fSourcePos.x + fSourceSize.x + fUV.x - fSize.x, fSourcePos.y + fSourceSize.y + fUV.y - fSize.y)/fTextureSize);
            }
            else if(fUV.x < fBorder.x) {
                color = texture(tex, vec2(fSourcePos.x + fUV.x, fSourcePos.y + mod(fUV.y, fSourceSize.y - fBorder.z - fBorder.w) + fBorder.z)/fTextureSize);
            }
            else if((fSize.x - fUV.x) < fBorder.y) {
                color = texture(tex, vec2(fSourcePos.x + fSourceSize.x + fUV.x - fSize.x, fSourcePos.y + mod(fUV.y, fSourceSize.y - fBorder.z - fBorder.w) + fBorder.z)/fTextureSize);
            }
            else if(fUV.y < fBorder.z) {
                color = texture(tex, vec2(fSourcePos.x + mod(fUV.x, fSourceSize.x - fBorder.x - fBorder.y) + fBorder.x, fSourcePos.y + fUV.y)/fTextureSize);
            }
            else if((fSize.y - fUV.y) < fBorder.w) {
                color = texture(tex, vec2(fSourcePos.x + mod(fUV.x, fSourceSize.x - fBorder.x - fBorder.y) + fBorder.x, fSourcePos.y + fSourceSize.y + fUV.y - fSize.y)/fTextureSize);
            }
            else {
                color = texture(tex, vec2(fSourcePos.x + mod(fUV.x, fSourceSize.x - fBorder.x - fBorder.y) + fBorder.x, fSourcePos.y + mod(fUV.y, fSourceSize.y - fBorder.z - fBorder.w) + fBorder.z)/fTextureSize);
            }
        }
        """

    def vertex_count(self):
        return 6

    def instanced(self):
        return True