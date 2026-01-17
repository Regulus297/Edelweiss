from PyQt5.QtCore import Qt, QPropertyAnimation, QTimer
from PyQt5.QtWidgets import QWidget, QLabel, QVBoxLayout, QApplication

class Popup(QWidget):
    def __init__(self, text, parent=None, duration=2000):
        super().__init__(parent)

        self.duration = duration
        self.setStyleSheet("background-color: #1f2329; color: white; padding: 10px;")

        self.setWindowFlags(Qt.FramelessWindowHint | Qt.Tool | Qt.WindowStaysOnTopHint)
        self.setAttribute(Qt.WA_TranslucentBackground)
        self.setWindowOpacity(0.0)
        
        label = QLabel(text)
        layout = QVBoxLayout(self)
        layout.addWidget(label)
        layout.setContentsMargins(0, 0, 0, 0)

        self.adjustSize()
        self.reposition()
        self.fadeIn()
        QTimer.singleShot(duration, self.fadeOut)

    def reposition(self):
        screen = QApplication.primaryScreen().availableGeometry()
        self.move(screen.right() - self.width() - 20, screen.bottom() - self.height() - 20)


    def fadeIn(self):
        self.anim = QPropertyAnimation(self, b"windowOpacity")
        self.anim.setDuration(300)
        self.anim.setStartValue(0.0)
        self.anim.setEndValue(1.0)
        self.anim.start()

    def fadeOut(self):
        self.anim = QPropertyAnimation(self, b"windowOpacity")
        self.anim.setDuration(300)
        self.anim.setStartValue(1.0)
        self.anim.setEndValue(0.0)
        self.anim.finished.connect(self.close)
        self.anim.start()