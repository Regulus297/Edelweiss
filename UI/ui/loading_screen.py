from PyQt5.QtCore import Qt
from PyQt5.QtWidgets import QWidget, QVBoxLayout

class LoadingScreen(QWidget):
    screens = {}

    def __init__(self, name, parent):
        super().__init__(parent)

        LoadingScreen.screens[name] = type(self)

        self.setAttribute(Qt.WA_NoSystemBackground, True)
        self.setAttribute(Qt.WA_TransparentForMouseEvents, False)
        self.setWindowFlags(Qt.FramelessWindowHint | Qt.SubWindow | Qt.WindowStaysOnTopHint)
        self.setStyleSheet("background-color: #1f2329;")
        self.progress = 0

        self.initUI()

    def initUI(self):
        ...
    @classmethod
    def create_from_name(cls, key, parent):
        return LoadingScreen.screens[key](parent)