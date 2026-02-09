from PyQt5.QtCore import Qt, pyqtSignal
from PyQt5.QtGui import QColor, QPainter
from PyQt5.QtWidgets import QWidget, QVBoxLayout, QPushButton, QHBoxLayout, QSizePolicy


# TODO: make this scrollable
class FormList(QWidget):
    itemAdded = pyqtSignal()
    itemRemoved = pyqtSignal(int)

    def __init__(self, generate_widget, parent=None):
        super().__init__(parent)
        self.generate_widget = generate_widget

        self.layout = QVBoxLayout()
        self.setLayout(self.layout)

        self.rows = []

        self.addButton = QPushButton("+")
        self.setSizePolicy(QSizePolicy.Policy.Expanding, QSizePolicy.Policy.Minimum)
        self.addButton.pressed.connect(self.new_row)
        self.layout.addWidget(self.addButton)
        self.layout.setAlignment(Qt.AlignTop)
        self.layout.itemAt(self.layout.count() - 1).setAlignment(Qt.AlignRight)
        self.setAttribute(Qt.WA_StyledBackground, True)
        self.layout.setSpacing(0)

        self.backgroundColor = QColor("#000")

    def new_row(self):
        self.itemAdded.emit()
        form = self.generate_widget(len(self.rows))
        self.addRow(form)

    def addRow(self, form):
        containerRow = QWidget(self)
        layout = QHBoxLayout()
        containerRow.setLayout(layout)
        layout.addWidget(form, stretch=5)
        removeButton = QPushButton("-")
        self.rows.append(form)
        removeButton.pressed.connect(lambda: self.remove_row(form))
        layout.addWidget(removeButton)
        self.layout.insertWidget(self.layout.count() - 1, containerRow)

    def remove_row(self, rowWidget):
        rowWidget.parent().close()
        self.itemRemoved.emit(self.rows.index(rowWidget))
        self.rows.remove(rowWidget)

    def clear(self):
        while len(self.rows) > 0:
            self.remove_row(self.rows[0])

    def paintEvent(self, a0):
        QPainter(self).fillRect(self.rect(), self.backgroundColor)
        super().paintEvent(a0)