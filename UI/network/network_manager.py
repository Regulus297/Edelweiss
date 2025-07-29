import inspect
import json
import os

import clr_loader
import pythonnet

from plugins import PluginLoader

pythonnet.load("coreclr")

import clr
from System.Reflection import Assembly
import System
from System import Activator, Int32, String

build_path: str = os.path.join(os.getcwd(), "Build")
clr.AddReference(os.path.join(build_path, "Newtonsoft.Json.dll"))


clr.AddReference(os.path.join(build_path, "Edelweiss.dll"))
from Edelweiss.Network import NetworkManager, Netcode, Packet
from plugins import JSONPreprocessor


from Edelweiss import Main

class PyNetworkManager:
    halt: bool = False

    receivers = {}

    @staticmethod
    def initialize():
        Main.Initialize()

    @staticmethod
    def update():
        while len(NetworkManager.queued) > 0:
            packet = NetworkManager.queued[0]

            if packet.code == Netcode.REGISTER_PYTHON_PLUGINS:
                data = JSONPreprocessor.loads(packet.data)
                for file in data["files"]:
                    PluginLoader.load_python_plugin(file)
            elif packet.code in PyNetworkManager.receivers.keys():
                for receiver in PyNetworkManager.receivers[packet.code]:
                    receiver.process_packet(packet)

            NetworkManager.DequeuePacket()
            break
        Main.Update()

    @staticmethod
    def send_packet(code: int, data: str):
        NetworkManager.ReceivePacket(Packet(System.Int64(code), data))

    @staticmethod
    def exit():
        Main.Exit()