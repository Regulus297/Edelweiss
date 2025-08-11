from PyQt5.QtCore import Qt, QTimer
from PyQt5.QtWidgets import QListWidget, QAbstractScrollArea, QSizePolicy, QAbstractItemView


class ResizingList(QListWidget):
    def __init__(self, parent=None, *itemInserted):
        super().__init__(parent)
        self.itemInserted = itemInserted
        self.keys = []
        self.setFocusPolicy(Qt.FocusPolicy.NoFocus)
        self.setContentsMargins(0, 0, 0, 0)
        self.setSelectionMode(QListWidget.SingleSelection)
        self.setUniformItemSizes(True)
        self.setHorizontalScrollBarPolicy(Qt.ScrollBarAlwaysOff)
        self.setVerticalScrollMode(QAbstractItemView.ScrollPerPixel)
        self.setSizeAdjustPolicy(QAbstractScrollArea.AdjustToContents)
        self.resize()
        self.max_height = 0

    def addKeyValuePair(self, key, value):
        self.keys.append(key)
        self.addItem(value)
    
    def addItem(self, aitem):
        super().addItem(aitem)
        QTimer.singleShot(0, self.resize)

    def addItems(self, items):
        super().addItems(items)
        QTimer.singleShot(0, self.resize)

    def resize(self):
        if self.sizePolicy().verticalPolicy() == QSizePolicy.Fixed:
            if self.count() == 0:
                self.hide()
                self.setFixedHeight(0)
                return
            else:
                self.show()

            totalHeight = self.sizeHintForRow(0) * self.count() + (self.frameWidth() * 2)
            self.setFixedHeight(self.clampedHeight(totalHeight))

        width = self.sizeHintForColumn(0)
        self.setMinimumWidth(width)


    def setMaximumHeight(self, height):
        self.max_height = height

    def clampedHeight(self, height):
        return height if self.max_height == 0 else min(height, self.max_height)

    def clear(self):
        super().clear()
        self.keys.clear()
        self.resize() 

    def setCurrentRow(self, row):
        index = 0
        if row in self.keys:
            index = self.keys.index(row)
        
        super().setCurrentRow(index)
