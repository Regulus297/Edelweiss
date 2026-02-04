import inspect
import json
import os

import clr_loader
import pythonnet


pythonnet.load("coreclr")

import clr

build_path: str = os.path.join(os.getcwd(), "Build/")
clr.AddReference(os.path.join(build_path, "Newtonsoft.Json.dll"))


clr.AddReference(os.path.join(build_path, "Edelweiss.dll"))
from Edelweiss import Main
from Edelweiss.Interop import InteropExtensions, ISyncable, PluginInterop

from plugins import PluginLoader

class Interop:
    halt: bool = False

    @staticmethod
    def initialize():
        Main.Initialize()
        PluginLoader.bind()

    @staticmethod
    def exit():
        Main.Exit()

    @staticmethod
    def addSyncCallback(name: str, callback):
        def event(syncName, syncable):
            if syncName == name:
                callback(syncable)
        InteropExtensions.OnSync += event

    @staticmethod
    def getSyncable(name: str):
        return ISyncable.Syncables[name]

    @staticmethod
    def getInterop(name: str):
        return PluginInterop.Interops[name]