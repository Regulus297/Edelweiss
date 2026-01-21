from PyQt5.QtCore import pyqtSignal, Qt
from PyQt5.QtWidgets import QWidget, QComboBox, QHBoxLayout, QPushButton


class ModifiableCombobox(QWidget):
    itemChanged = pyqtSignal(str)
    itemAdded = pyqtSignal(str)
    itemEdited = pyqtSignal(str, str)
    itemRemoved = pyqtSignal(str)

    def __init__(self, default_items, parent=None, removable_items = None):
        super().__init__(parent)
        self.default_items = default_items
        self.removable_items = removable_items if removable_items else set()

        self.layout = QHBoxLayout()
        self.setLayout(self.layout)

        self.combobox = QComboBox()
        self.combobox.setEditable(True)
        self.combobox.addItems(self.default_items)
        self.combobox.setInsertPolicy(QComboBox.NoInsert)
        self.layout.addWidget(self.combobox, stretch=5)

        self.add_button = QPushButton("+")
        self.add_button.pressed.connect(self._new_item)
        self.layout.addWidget(self.add_button)

        self.remove_button = QPushButton("-")
        self.remove_button.pressed.connect(self.remove_item)
        self.remove_button.setEnabled(False)
        self.layout.addWidget(self.remove_button)

        self.combobox.currentIndexChanged.connect(self.indexChanged)
        self.combobox.lineEdit().editingFinished.connect(self.editCurrentItem)

        self._canEditDefaults = True

    @property
    def canEditDefaults(self):
        return self._canEditDefaults

    @canEditDefaults.setter
    def canEditDefaults(self, value):
        self._canEditDefaults = value
        if self.combobox.currentText() not in self.removable_items:
            self.combobox.lineEdit().setEnabled(False)

    def editCurrentItem(self):
        index = self.combobox.currentIndex()
        if index < 0:
            return

        new_text = self.combobox.lineEdit().text()
        old_text = self.combobox.itemText(index)

        if not new_text or new_text == old_text:
            return

        if new_text in (self.combobox.itemText(i) for i in range(self.combobox.count())):
            self.combobox.setItemText(index, old_text)
            return

        if old_text in self.removable_items:
            self.removable_items.remove(old_text)
            self.removable_items.add(new_text)

        self.combobox.setItemText(index, new_text)
        self.itemEdited.emit(old_text, new_text)

    def indexChanged(self, i):
        if self.combobox.currentText() not in self.removable_items:
            self.remove_button.setEnabled(False)
            self.setEditable(self._canEditDefaults)
        else:
            self.remove_button.setEnabled(True)
            self.setEditable(True)
        self.itemChanged.emit(self.combobox.itemText(i))

    def _new_item(self):
        index = 1
        for i in range(self.combobox.count()):
            text = self.combobox.itemText(i)
            if text.startswith("Item ") and text[5:].isnumeric():
                index = max(int(text[5:]) + 1, index)

        self.addItem(f"Item {index}")
        self.itemAdded.emit(f"Item {index}")

    def addItem(self, text):
        if not text or text in [self.combobox.itemText(i) for i in range(self.combobox.count())]:
            return
        self.combobox.addItem(text)
        self.removable_items.add(text)
        self.combobox.setCurrentIndex(self.combobox.count() - 1)

    def remove_item(self):
        index = self.combobox.currentIndex()
        text = self.combobox.currentText()

        if text in self.removable_items:
            self.removable_items.remove(text)
            self.combobox.removeItem(index)

        self.itemRemoved.emit(text)

    def setEditable(self, editable):
        self.combobox.lineEdit().setEnabled(editable)
        if editable:
            self.combobox.setFocusPolicy(Qt.FocusPolicy.StrongFocus)
        else:
            self.combobox.setFocusPolicy(Qt.FocusPolicy.NoFocus)
            self.combobox.clearFocus()