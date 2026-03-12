import os

from PyQt5.QtCore import pyqtSignal
from PyQt5.QtGui import QIcon
from PyQt5.QtWidgets import QWidget, QHBoxLayout, QPushButton, QLineEdit, QFileDialog, QSizePolicy

from ..pixmap_loader import PixmapLoader


class FileLineEdit(QWidget):
    editingFinished = pyqtSignal(str)

    def __init__(self, fileDialogTitle, filter="", directory=True, parent=None):
        super(FileLineEdit, self).__init__(parent)

        self.fileDialogTitle = fileDialogTitle
        self.filter = filter
        self.directory = directory

        self.layout = QHBoxLayout()
        self.setLayout(self.layout)

        self.filepathLineEdit = QLineEdit()
        self.layout.addWidget(self.filepathLineEdit)
        self.filepathLineEdit.editingFinished.connect(self.editingFinished.emit)

        self.browseButton = QPushButton()
        self.browseButton.setIcon(QIcon(PixmapLoader.load_texture("Edelweiss/browse_icon")))
        self.browseButton.clicked.connect(self.onBrowse)
        self.layout.addWidget(self.browseButton)

    def setText(self, text):
        self.filepathLineEdit.setText(text)

    def text(self):
        return self.filepathLineEdit.text()

    def onBrowse(self):
        if self.directory:
            file = str(QFileDialog.getExistingDirectory(self, self.fileDialogTitle, self.text()))
        else:
            file = str(QFileDialog.getOpenFileName(self, self.fileDialogTitle, self.text(), self.filter)[0])
        self.filepathLineEdit.setText(file)
        self.editingFinished.emit(file)

    def getLineEdit(self):
        return self.filepathLineEdit

    def getBrowseButton(self):
        return self.browseButton

    def setIcon(self, path):
        if os.path.isfile(path):
            self.browseButton.setIcon(QIcon(path))

    def setTitle(self, value):
        self.fileDialogTitle = value