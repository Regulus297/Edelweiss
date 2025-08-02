from PyQt5.QtCore import pyqtSignal, Qt
from PyQt5.QtWidgets import QListWidget


class ResizingList(QListWidget):
    def __init__(self, parent=None, *itemInserted):
        super().__init__(parent)
        self.itemInserted = itemInserted
        self.keys = []
        self.setFocusPolicy(Qt.FocusPolicy.NoFocus)
        self.setSelectionMode(QListWidget.SingleSelection)

    def addKeyValuePair(self, key, value):
        self.keys.append(key)
        self.addItem(value)
    
    def addItem(self, aitem):
        super().addItem(aitem)
        totalHeight = 27 * self.count() + 11
        self.setFixedHeight(totalHeight)

        width = self.sizeHintForColumn(0)
        self.setMinimumWidth(width + 60)

    def clear(self):
        super().clear()
        self.setFixedHeight(0)

    def setCurrentRow(self, row):
        index = 0
        if row in self.keys:
            index = self.keys.index(row)
        
        super().setCurrentRow(index)
