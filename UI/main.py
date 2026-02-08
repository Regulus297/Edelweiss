import logging
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

from ui import MainWindow

from interop import Interop


def subscribe_to_pref(pref):
    mainInterop = Interop.getInterop("Edelweiss:MainInterop")
    pref.OnFailedFileLoad += lambda: mainInterop.SetCelesteDirectory(input("Enter Celeste Directory: "))

if __name__ == '__main__':
    Interop.addSyncCallback("Edelweiss:CelesteDirectoryPref", subscribe_to_pref)

    def excepthook(exc_type, exc_value, exc_tb):
        logging.critical(
            "Uncaught exception",
            exc_info=(exc_type, exc_value, exc_tb)
        )


    logging.basicConfig(
        filename="crashlog.txt",
        level=logging.ERROR,
        format="%(asctime)s - %(levelname)s - %(message)s"
    )

    sys.excepthook = excepthook

    Interop.initialize()
    app = QApplication(sys.argv)
    app.setDoubleClickInterval(200)
    window = MainWindow()
    sys.exit(app.exec_())
