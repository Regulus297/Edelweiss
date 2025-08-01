from PyQt5.QtCore import pyqtSignal, Qt
from PyQt5.QtWidgets import QListWidget


class ResizingList(QListWidget):
    def __init__(self, parent=None, *itemInserted):
        super().__init__(parent)
        self.itemInserted = itemInserted
        self.setFocusPolicy(Qt.FocusPolicy.NoFocus)
        self.setSelectionMode(QListWidget.SingleSelection)

    def addItem(self, aitem):
        super().addItem(aitem)
        totalHeight = 27 * self.count() + 11
        self.setFixedHeight(totalHeight)

        width = self.sizeHintForColumn(0)
        self.setMinimumWidth(width + 60)

    def clear(self):
        super().clear()
        self.setFixedHeight(0)
