from plugins import plugin_loadable, UtilJsonFunction, load_dependencies, JSONPreprocessor
from network import SyncedVariables
from ui import MappingWindow
from Edelweiss.Network import Netcode
from Edelweiss.Plugins import PluginLoader
from PyQt5.QtCore import Qt
import sys


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
    

@load_dependencies("deferred_value.py")
@plugin_loadable
class DeferFunction(UtilJsonFunction):
    def __init__(self):
        super().__init__("defer")

    def call(self, *args) -> object:
        value = DeferredValue(lambda: JSONPreprocessor.preprocess(args[0]))
        return value
    

@load_dependencies("widgets/zoomable_view.py")
@plugin_loadable
class PenWidthFunction(UtilJsonFunction):
    def __init__(self):
        super().__init__("pen_thickness")
    
    def call(self, *args) -> object:
        widget = MappingWindow.instance.get_tracked_widget(args[0])
        if type(widget) != ZoomableView:
            return 0
        
        return widget.pen_size


@plugin_loadable
class WidgetPropertyFunction(UtilJsonFunction):
    def __init__(self):
        super().__init__("widget_property")

    def call(self, *args) -> object:
        if len(args) != 2:
            return
        
        widget = MappingWindow.instance.get_tracked_widget(args[0])
        if widget is not None:
            try:
                return eval(f"widget.{args[1]}")
            except Exception as e:
                print("Error while getting widget property:")
                return 
            

@plugin_loadable
class AlignmentFunction(UtilJsonFunction):
    def __init__(self):
        super().__init__("alignment")

    def call(self, *args) -> object:
        if len(args) != 1:
            align = 0
            for arg in args:
                align |= self.call(arg)
            return align
        
        if args[0] == "right":
            return Qt.AlignRight
        elif args[0] == "top":
            return Qt.AlignTop
        elif args[0] == "bottom":
            return Qt.AlignBottom
        elif args[0] == "center":
            return Qt.AlignCenter
        elif args[0] == "vCenter":
            return Qt.AlignVCenter
        elif args[0] == "hCenter":
            return Qt.AlignHCenter
        return Qt.AlignLeft
    

@plugin_loadable
class MaxFloatFunction(UtilJsonFunction):
    def __init__(self):
        super().__init__("maxfloat")

    def call(self, *args) -> object:
        return sys.float_info.max
    

@plugin_loadable
class MinFloatFunction(UtilJsonFunction):
    def __init__(self):
        super().__init__("minfloat")

    def call(self, *args) -> object:
        return sys.float_info.min
    

@plugin_loadable
class MaxIntFunction(UtilJsonFunction):
    def __init__(self):
        super().__init__("maxint")

    def call(self, *args) -> object:
        return sys.maxint
    

@plugin_loadable
class MinIntFunction(UtilJsonFunction):
    def __init__(self):
        super().__init__("minint")

    def call(self, *args) -> object:
        return sys.min_int
    

@plugin_loadable
class GetVarFunction(UtilJsonFunction):
    def __init__(self):
        super().__init__("getVar")

    def call(self, *args) -> object:
        if len(args) != 1:
            return ""
        
        return SyncedVariables.variables[args[0]]

@plugin_loadable
class GetTextFunction(UtilJsonFunction):
    def __init__(self):
        super().__init__("getText")

    def call(self, *args) -> object:
        if len(args) != 1:
            return ""
        
        language = SyncedVariables.variables["Language"]
        var = PluginLoader.localization[language]
        if args[0] in var:
            return var[args[0]]
        return args[0]