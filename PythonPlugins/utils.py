from plugins import plugin_loadable, UtilJsonFunction, load_dependencies, JSONPreprocessor
from ui import MappingWindow
from Edelweiss.Network import Netcode
import random


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
                print(e)
                return 