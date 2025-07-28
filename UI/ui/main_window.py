import json
from typing import List

from PyQt5.QtCore import Qt, QTimer
from PyQt5.QtGui import QCursor
from PyQt5.QtWidgets import QWidget, QVBoxLayout, QGraphicsView, QGraphicsScene, QMenuBar, QPushButton, QApplication, \
    QSplitter, QListWidget, QSizePolicy, QHBoxLayout, QLineEdit, QListWidgetItem, QTabWidget, QStackedWidget, \
    QSpacerItem, QToolBar, QMainWindow, QAction, QFrame, QComboBox

from network.network_manager import PyNetworkManager


class MappingWindow(QMainWindow):
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

        self.tab_switcher.addItem(name)
        self.resize_tab_switcher(self.tab_switcher.currentText())
        # self.top_bar_layout.insertWidget(self.top_bar_layout.count() - 1, button)

    def initUI(self):
        self.setGeometry(200, 200, 800, 600)
        self.setWindowTitle("Edelweiss")

        central = QWidget()
        self.setCentralWidget(central)

        self.layout = QVBoxLayout()
        central.setLayout(self.layout)
        self.layout.setContentsMargins(0, 0, 0, 0)

        self.stack = QStackedWidget()
        self.layout.addWidget(self.stack)
        self.stack.setSizePolicy(QSizePolicy.Expanding, QSizePolicy.Expanding)

        self.tool_bar = QToolBar()
        self.addToolBar(self.tool_bar)


        self.tab_switcher = QComboBox()
        self.tab_switcher.currentTextChanged.connect(self.resize_tab_switcher)


        self.tool_bar.addWidget(self.tab_switcher)



        with open("stylesheet.qss", "r") as f:
            self.setStyleSheet(f.read())

        self.showMaximized()

    def resize_tab_switcher(self, text):
        fm = self.tab_switcher.fontMetrics()
        text_width = fm.width(text)
        self.tab_switcher.setFixedWidth(text_width + 40)