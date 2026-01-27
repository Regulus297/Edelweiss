import inspect
import json
import os

import clr_loader
import pythonnet

pythonnet.load("coreclr")

import clr
from System.Reflection import Assembly
import System
from System import Activator, Int32, String, Object

build_path: str = os.path.join(os.getcwd(), "Build/")
clr.AddReference(os.path.join(build_path, "Newtonsoft.Json.dll"))


clr.AddReference(os.path.join(build_path, "Edelweiss.dll"))
from Edelweiss.MVC import Model, Syncable
from Edelweiss import Main

class MVC:
    @staticmethod
    def initialize():
        Main.Initialize()

    @staticmethod
    def post_load():
        Main.PostLoad()

    @staticmethod
    def exit():
        Main.Exit()

    @staticmethod
    def addModelCreationCallback(modelType: str, callback):
        def event(mType, model):
            if mType.Name == modelType:
                callback(model)
        Model.ModelCreated += event

    @staticmethod
    def addBaseTypeCreationCallback(baseType: str, callback):
        def event(mType, model):
            while mType.BaseType is not None:
                mType = mType.BaseType
                if mType.Name == baseType:
                    callback(model)
                    break
        Model.ModelCreated += event

    @staticmethod
    def addSyncCallback(syncName: str, callback):
        def event(name, syncable):
            if name == syncName:
                callback(syncable)

        Syncable.OnSync += event

    @staticmethod
    def getSyncable(name: str):
        return Syncable.syncables[name]