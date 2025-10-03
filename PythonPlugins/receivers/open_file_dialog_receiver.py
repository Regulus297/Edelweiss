from network import PacketReceiver, PyNetworkManager
from plugins import plugin_loadable, load_dependencies, JSONPreprocessor, get_event_data
from ui import MappingWindow, JSONWidgetLoader
from Edelweiss.Network import Netcode
from PyQt5.QtWidgets import QFileDialog
import json


@plugin_loadable
class OpenFileDialogReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.OPEN_FILE_DIALOG)

    def process_packet(self, packet):
        data = JSONPreprocessor.loads(packet.data)
        file = data["file"]
        directory = data["path"] if "path" in data else ""
        _filter = data["pattern"] if "pattern" in data else ""
        existing = data["mode"] == "load"
        netcode, extra = get_event_data(data["submit"])
        
        if file:
            if existing:
                path, pattern = QFileDialog.getOpenFileName(directory = directory, filter = _filter)
            else:
                path, pattern = QFileDialog.getSaveFileName(directory = directory, filter = _filter)
        else:
            path, pattern = QFileDialog.getExistingDirectory(directory = directory), ""
        
        if path == "":
            return
        PyNetworkManager.send_packet(netcode, json.dumps({
            "path": path,
            "pattern": pattern,
            "extraData": extra
        }))
