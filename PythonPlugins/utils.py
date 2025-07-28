from plugins.plugin_loadable import plugin_loadable
from plugins.util_json_function import UtilJsonFunction
from Edelweiss.Network import Netcode


@plugin_loadable
class NetcodeFunction(UtilJsonFunction):
    def __init__(self):
        super().__init__("netcode")

    def call(self, *args) -> object:
        if len(args) == 0:
            return Netcode.NONE
        
        if type(args[0]) != str:
            return Netcode.NONE
        
        return Netcode.Get(args[0])