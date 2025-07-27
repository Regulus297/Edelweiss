import json
from typing import List

from PyQt5.QtCore import Qt, QTimer
from PyQt5.QtGui import QCursor
from PyQt5.QtWidgets import QWidget, QVBoxLayout, QGraphicsView, QGraphicsScene, QMenuBar, QPushButton, QApplication, \
    QSplitter, QListWidget, QSizePolicy, QHBoxLayout, QLineEdit, QListWidgetItem, QTabWidget, QStackedWidget, \
    QSpacerItem

from network.network_manager import PyNetworkManager


class MappingWindow(QWidget):
    instance = None

    scene_widgets = {"QGraphicsView": QGraphicsView}

    def __init__(self, parent=None):
        super().__init__(parent)
        MappingWindow.instance = self


        self.timer = QTimer()
        self.timer.timeout.connect(PyNetworkManager.update)
        self.timer.start(16)

        self.initUI()

    def register_scene_json(self, data):
        data = json.loads(data)
        widget = MappingWindow.scene_widgets[data["type"]](parent = self)
        self.register_scene(widget, data["name"])

    def register_scene(self, widget, name):
        self.stack.addWidget(widget)

        button = QPushButton(name)
        button.setSizePolicy(QSizePolicy.Fixed, QSizePolicy.Fixed)

        self.top_bar.insertWidget(self.top_bar.count() - 1, button)

    def initUI(self):
        self.setGeometry(200, 200, 800, 600)
        self.setWindowTitle("Edelweiss")


        self.layout = QVBoxLayout()
        self.setLayout(self.layout)
        self.layout.setContentsMargins(10, 10, 10, 10)

        self.top_bar = QHBoxLayout()
        self.layout.addLayout(self.top_bar)

        self.top_bar.addItem(QSpacerItem(0, 0, QSizePolicy.Expanding, QSizePolicy.Fixed))

        self.stack = QStackedWidget()
        self.layout.addWidget(self.stack)
        self.stack.setSizePolicy(QSizePolicy.Expanding, QSizePolicy.Expanding)



        with open("stylesheet.qss", "r") as f:
            self.setStyleSheet(f.read())

        self.showMaximized()
