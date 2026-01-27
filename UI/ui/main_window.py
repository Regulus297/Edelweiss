import json

from PyQt5.QtCore import  QTimer
from PyQt5.QtWidgets import (QWidget, QVBoxLayout, QGraphicsView, QSizePolicy, QStackedWidget, QToolBar,
                             QMainWindow, QComboBox, QSplashScreen)

from mvc import MVC
from utils import System


class MainWindow(QMainWindow):
    instance = None
    stylesheet = ""

    def __init__(self, parent=None):
        super().__init__(parent)
        MainWindow.instance = self

        self.trackedWidgets = {}
        self.tab_model_data = None

        self.set_tab_model()
        self.initUI()

    def get_tracked_widget(self, key):
        if key in self.trackedWidgets.keys():
            return self.trackedWidgets[key]
        return None

    # def register_scene_json(self, data):
    #     data = JSONPreprocessor.loads(data)
    #     widget = MappingWindow.scene_widgets[data["type"]](parent = self)
    #     self.register_scene(widget, data["name"], data["internalName"])

    def register_scene(self, model):
        # self.stack.addWidget(widget)

        self.tabs.append(model.Get("InternalName"))
        self.tab_switcher.addItem(model.Get("DisplayName"))
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

        self.loadingScreen = None

        self.tab_switcher = QComboBox()
        self.tab_switcher.currentTextChanged.connect(self.on_tab_switched)

        self.tool_bar.addWidget(self.tab_switcher)

        with open("stylesheet.qss", "r") as f:
            MainWindow.stylesheet = f.read()
            self.setStyleSheet(MainWindow.stylesheet)

        self.showMaximized()

    def on_tab_switched(self, text):
        self.tab_model_data.Controller.SetField("Tab", self.current_tab)
        # Resize box to fit text
        fm = self.tab_switcher.fontMetrics()
        text_width = fm.width(text)
        self.tab_switcher.setMinimumWidth(text_width + 60)

        # Set the index of the stackedwidget
        self.stack.setCurrentIndex(self.tab_switcher.currentIndex())

    @property
    def current_tab(self):
        return self.tabs[self.tab_switcher.currentIndex()]


    def closeEvent(self, a0):
        MVC.exit()
        super().closeEvent(a0)

    def set_tab_model(self):
        self.tab_model_data = MVC.getSyncable("Edelweiss:TabModelData")
        self.tab_model_data.Subscribe("TabRegistered", System.Action[object](self.register_scene))