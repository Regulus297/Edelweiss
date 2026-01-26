import os.path
from idlelib.delegator import Delegator

if not os.path.exists(os.path.join(os.getcwd(), "Build/Edelweiss.dll")):
    os.chdir("../")


from mvc import MVC
from utils import System

def subscribe_to_model(model):
    model.Subscribe("OnFailedFileLoad", System.Action(lambda: model.Controller.SetField("Value", input("Enter Celeste Directory: "))))

MVC.addModelCreationCallback("CelesteDirectoryPref", subscribe_to_model)
MVC.initialize()

MVC.exit()