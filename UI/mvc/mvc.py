import inspect
import json
import os

import clr_loader
import pythonnet

pythonnet.load("coreclr")

import clr
from System.Reflection import Assembly
import System
from System import Activator, Int32, String

build_path: str = os.path.join(os.getcwd(), "Build/")
clr.AddReference(os.path.join(build_path, "Newtonsoft.Json.dll"))


clr.AddReference(os.path.join(build_path, "Edelweiss.dll"))
from Edelweiss.MVC import Model
from Edelweiss import Main

class MVC:
    halt: bool = False

    @staticmethod
    def initialize():
        Main.Initialize()

    @staticmethod
    def exit():
        Main.Exit()

    @staticmethod
    def addModelCreationCallback(modelType: str, callback):
        def event(mType, model):
            if mType.Name == modelType:
                callback(model)
        Model.ModelCreated += event