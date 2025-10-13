from PyQt5.QtCore import Qt, QEvent, QTimer
from PyQt5.QtGui import QPainter, QMouseEvent, QBrush, QColor
from PyQt5.QtWidgets import QLabel, QVBoxLayout
from plugins import plugin_loadable, load_dependencies
from ui import LoadingScreen, PixmapLoader


@plugin_loadable
class MainLoadingScreen(LoadingScreen):
    def __init__(self, parent=None):
        super().__init__("main", parent)
        self.empty_pixmap = None
        self.fill_pixmap = None
    
    def initUI(self):
        QTimer.singleShot(1, self.initDelayed)

    def initDelayed(self):
        self.empty_pixmap = PixmapLoader.load_texture("Edelweiss/unfilled")
        self.fill_pixmap = PixmapLoader.load_texture("Edelweiss/fill")
        self.layout = QVBoxLayout()
        self.layout.setContentsMargins(0, 0, 0, 0)
        self.setLayout(self.layout)


    def paintEvent(self, event):
        painter = QPainter(self)
        painter.setBrush(QBrush(QColor("#1f2329")))
        painter.drawRect(self.rect())
        if self.empty_pixmap:
            x = (self.width() - self.empty_pixmap.width()) // 2
            y = (self.height() - self.empty_pixmap.height()) // 2
            painter.drawPixmap(x, y, self.empty_pixmap)
        if self.fill_pixmap:
            x = (self.width() - self.fill_pixmap.width()) // 2
            y = (self.height() - self.fill_pixmap.height()) // 2
            sy = int(self.fill_pixmap.height() * (1-self.progress))
            painter.drawPixmap(x, y + sy, self.fill_pixmap, 0, sy, self.fill_pixmap.width(), self.fill_pixmap.height() - sy)