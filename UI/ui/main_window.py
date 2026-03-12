import json

from PyQt5.QtCore import  QTimer
from PyQt5.QtWidgets import (QWidget, QVBoxLayout, QGraphicsView, QSizePolicy, QStackedWidget, QToolBar,
                             QMainWindow, QComboBox, QSplashScreen, QWidgetAction)

from interop import Interop, SyncableProperty, InteropMethod
from .json_toolbar_loader import JSONToolbarLoader
from .widget_binding import WidgetBinding
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
        self.tool_bar.addWidget(self.tab_switcher)

        self._tab_switch_method = InteropMethod("Edelweiss:MainInterop.ChangeTab")
        self._switch_event = SyncableProperty("Edelweiss.TabSelected", sync=False).get()
        self._switch_event += self.refresh_toolbar
        self._switcher_binding = SyncableProperty("Edelweiss.Tabs", ItemAdded=self.register_tab)
        self._enabled_binding = SyncableProperty("Edelweiss:ModdingTab.HasCurrentMod", ValueChanged=self.tab_switcher.setEnabled)

        with open("stylesheet.qss", "r") as f:
            MainWindow.stylesheet = f.read()
            self.setStyleSheet(MainWindow.stylesheet)

        self.showMaximized()

    def refresh_toolbar(self, tab):
        for action in self.tool_bar.actions():
            if isinstance(action, QWidgetAction) and action.defaultWidget() == self.tab_switcher:
                continue
            self.tool_bar.removeAction(action)
        self.tool_bar.addSeparator()

        JSONToolbarLoader.init_toolbar(self.tool_bar, json.loads(tab.ToolbarWidget))

    @property
    def current_tab(self):
        return self.tabs[self.tab_switcher.currentIndex()]

    def on_tab_switched(self, _):
        self._tab_switch_method(self.current_tab)
        self.stack.setCurrentIndex(self.tab_switcher.currentIndex())

    def closeEvent(self, a0):
        Interop.exit()
        super().closeEvent(a0)

    def resizeEvent(self, a0):
        if self.loadingScreen:
            self.loadingScreen.setGeometry(self.rect())