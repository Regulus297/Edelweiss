import os.path
from idlelib.delegator import Delegator

if not os.path.exists(os.path.join(os.getcwd(), "Build/Edelweiss.dll")):
    os.chdir("../")


from interop import Interop
from utils import System

def subscribe_to_pref(pref):
    mainInterop = Interop.getInterop("Edelweiss:MainInterop")
    pref.OnFailedFileLoad += lambda: mainInterop.SetCelesteDirectory(input("Enter Celeste Directory: "))

Interop.addSyncCallback("Edelweiss:CelesteDirectoryPref", subscribe_to_pref)
Interop.initialize()

Interop.exit()