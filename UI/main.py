import os.path
import sys

if not os.path.exists(os.path.join(os.getcwd(), "Build/Edelweiss.dll")):
    os.chdir("../")

from PyQt5.QtWidgets import QApplication

from ui.main_window import MappingWindow



from network.network_manager import PyNetworkManager

PyNetworkManager.initialize()


if __name__ == '__main__':
    app = QApplication(sys.argv)
    window = MappingWindow()
    sys.exit(app.exec_())


PyNetworkManager.exit()