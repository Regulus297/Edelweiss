import json

from PyQt5.QtCore import  QTimer
from PyQt5.QtWidgets import (QWidget, QVBoxLayout, QGraphicsView, QSizePolicy, QStackedWidget, QToolBar,
                             QMainWindow, QComboBox, QSplashScreen)

from interop import Interop, ListBinding, SyncableProperty, InteropMethod
from .json_widget_loader import JSONWidgetLoader


class MainWindow(QMainWindow):
    instance = None
    stylesheet = ""

    def __init__(self, parent=None):
        super().__init__(parent)
        MainWindow.instance = self

        self.trackedWidgets = {}

        self.initUI()

    def get_tracked_widget(self, key):
        if key in self.trackedWidgets.keys():
            return self.trackedWidgets[key]
        return None

    def register_tab(self, tab):
        widget = JSONWidgetLoader.init_widget(json.loads(tab.LayoutWidget))
        self.stack.addWidget(widget)

        self.tabs.append(tab)
        self.tab_switcher.addItem(tab.DisplayName)
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

        self._tab_switch_method = InteropMethod("Edelweiss:MainInterop.ChangeTab")
        self._switcher_binding = ListBinding(SyncableProperty("Edelweiss.Tabs"), None, self.register_tab, None, None)


        self.tool_bar.addWidget(self.tab_switcher)

        with open("stylesheet.qss", "r") as f:
            MainWindow.stylesheet = f.read()
            self.setStyleSheet(MainWindow.stylesheet)

        self.showMaximized()

    @property
    def current_tab(self):
        return self.tabs[self.tab_switcher.currentIndex()]

    def on_tab_switched(self, text):
        self._tab_switch_method(self.current_tab)

    def closeEvent(self, a0):
        Interop.exit()
        super().closeEvent(a0)

    def resizeEvent(self, a0):
        if self.loadingScreen:
            self.loadingScreen.setGeometry(self.rect())