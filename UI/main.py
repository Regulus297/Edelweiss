import os.path
import sys

from PyQt5.QtWidgets import QApplication


if not os.path.exists(os.path.join(os.getcwd(), "Build/Edelweiss.dll")):
    os.chdir("../")


from mvc import MVC
from ui.main_window import MainWindow
from utils import System

def subscribe_to_model(model):
    model.Subscribe("OnFailedFileLoad", System.Action(lambda: model.Controller.SetField("Value", input("Enter Celeste Directory: "))))



if __name__ == '__main__':
    app = QApplication(sys.argv)
    MVC.addModelCreationCallback("CelesteDirectoryPref", subscribe_to_model)
    MVC.initialize()
    window = MainWindow()
    MVC.post_load()
    sys.exit(app.exec())

MVC.exit()