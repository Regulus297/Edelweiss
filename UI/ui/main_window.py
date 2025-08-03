import json
from typing import List

from PyQt5.QtCore import Qt, QTimer
from PyQt5.QtGui import QCursor, QDoubleValidator, QIntValidator
from PyQt5.QtWidgets import QWidget, QVBoxLayout, QGraphicsView, QGraphicsScene, QMenuBar, QPushButton, QApplication, \
    QSplitter, QListWidget, QSizePolicy, QHBoxLayout, QLineEdit, QListWidgetItem, QTabWidget, QStackedWidget, \
    QSpacerItem, QToolBar, QMainWindow, QAction, QFrame, QComboBox, QMenu, QWidgetAction, QCheckBox, QGridLayout, QLabel

from network import PyNetworkManager
from Edelweiss.Network import Netcode
from plugins import JSONPreprocessor


class MappingWindow(QMainWindow):
    instance = None
    stylesheet = ""

    scene_widgets = {"QGraphicsView": QGraphicsView}

    def __init__(self, parent=None):
        super().__init__(parent)
        MappingWindow.instance = self

        self.trackedWidgets = {}


        self.timer = QTimer()
        self.timer.timeout.connect(PyNetworkManager.update)
        self.timer.start(1)

        self.initUI()

    def get_tracked_widget(self, key):
        if key in self.trackedWidgets.keys():
            return self.trackedWidgets[key]
        return None

    def register_scene_json(self, data):
        data = JSONPreprocessor.loads(data)
        widget = MappingWindow.scene_widgets[data["type"]](parent = self)
        self.register_scene(widget, data["name"], data["internalName"])

    def register_scene(self, widget, name, internalName):
        self.stack.addWidget(widget)

        self.tabs.append(internalName)
        self.tab_switcher.addItem(name)
        self.on_tab_switched(self.tab_switcher.currentText())

    def initUI(self):
        self.setGeometry(200, 200, 800, 600)
        self.setWindowTitle("Edelweiss")

        self.tabs = []

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
        self.tab_switcher.currentTextChanged.connect(self.on_tab_switched)


        self.tool_bar.addWidget(self.tab_switcher)

        with open("stylesheet.qss", "r") as f:
            MappingWindow.stylesheet = f.read()
            self.setStyleSheet(MappingWindow.stylesheet)

        self.showMaximized()

    def on_tab_switched(self, text):
        # Resize box to fit text
        fm = self.tab_switcher.fontMetrics()
        text_width = fm.width(text)
        self.tab_switcher.setMinimumWidth(text_width + 60)

        # Notify backend
        PyNetworkManager.send_packet(Netcode.TAB_CHANGED, json.dumps({
           "prev": self.tabs[self.stack.currentIndex()],
            "curr": self.tabs[self.tab_switcher.currentIndex()]
        }))

        # Set the index of the stackedwidget
        self.stack.setCurrentIndex(self.tab_switcher.currentIndex())

    @property
    def current_tab(self):
        return self.tabs[self.stack.currentIndex()]

    def closeEvent(self, a0):
        PyNetworkManager.exit()
        super().closeEvent(a0)