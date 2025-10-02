import os.path
import subprocess
import sys

if os.path.exists(os.path.join(os.getcwd(), "Edelweiss.dll")):
    # Uncompressed, so decompress the file
    print("Decompressing!")
    subprocess.run("./ProjectDecompressor.exe Edelweiss.dll")
    os.remove("Edelweiss.dll")

if not os.path.exists(os.path.join(os.getcwd(), "Build/Edelweiss.dll")):
    os.chdir("../")

from PyQt5.QtWidgets import QApplication

from ui import MappingWindow

from network import PyNetworkManager

PyNetworkManager.initialize()


if __name__ == '__main__':
    app = QApplication(sys.argv)
    app.setDoubleClickInterval(200)
    window = MappingWindow()
    sys.exit(app.exec_())

PyNetworkManager.exit()