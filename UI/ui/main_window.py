import json
from typing import List

from PyQt5.QtCore import Qt, QTimer
from PyQt5.QtGui import QCursor
from PyQt5.QtWidgets import QWidget, QVBoxLayout, QGraphicsView, QGraphicsScene, QMenuBar, QPushButton, QApplication, \
    QSplitter, QListWidget, QSizePolicy, QHBoxLayout, QLineEdit, QListWidgetItem

from network.network_manager import PyNetworkManager
from ui.mapping_view import MappingView
from ui.scene import Scene


class MappingWindow(QWidget):
    instance = None
    def __init__(self, parent=None):
        super().__init__(parent)
        MappingWindow.instance = self

        self.timer = QTimer()
        self.timer.timeout.connect(PyNetworkManager.update)
        self.timer.start(16)

        self.initUI()



    def initUI(self):
        self.setGeometry(200, 200, 800, 600)
        self.setWindowTitle("Edelweiss")


        self.scene = Scene()
        self.grScene = self.scene.grScene

        self.grView = MappingView(self.grScene, self)

        self.layout = QVBoxLayout()
        self.setLayout(self.layout)
        self.layout.setContentsMargins(0, 0, 0, 0)

        self.layout.addWidget(self.grView)

        with open("stylesheet.qss", "r") as f:
            self.setStyleSheet(f.read())

        self.showMaximized()
