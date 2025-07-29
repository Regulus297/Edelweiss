from ui import JSONWidgetLoader, CommonPropertySetter, MappingWindow
from plugins import plugin_loadable, load_dependencies


@plugin_loadable
class IDPropertySetter(CommonPropertySetter):
    def __init__(self):
        super().__init__("id")

    def set_property(self, widget, property_value):
        widget.setObjectName(property_value)


@load_dependencies("common_code.py")
@plugin_loadable
class SizePolicyXSetter(CommonPropertySetter):
    def __init__(self):
        super().__init__("sizePolicyX")

    def set_property(self, widget, property_value):
        policy = widget.sizePolicy()
        policy.setHorizontalPolicy(get_size_policy(property_value))
        widget.setSizePolicy(policy)


@load_dependencies("common_code.py")
@plugin_loadable
class SizePolicyYSetter(CommonPropertySetter):
    def __init__(self):
        super().__init__("sizePolicyY")

    def set_property(self, widget, property_value):
        policy = widget.sizePolicy()
        policy.setVerticalPolicy(get_size_policy(property_value))
        widget.setSizePolicy(policy)


@plugin_loadable
class StylesheetSetter(CommonPropertySetter):
    def __init__(self):
        super().__init__("stylesheet")

    def set_property(self, widget, property_value):
        widget.setStyleSheet(property_value)


@plugin_loadable
class TrackedAsSetter(CommonPropertySetter):
    def __init__(self):
        super().__init__("trackedAs")

    def set_property(self, widget, property_value):
        MappingWindow.instance.trackedWidgets[property_value] = widget
        setattr(widget, "__tracked_as__", property_value)


@plugin_loadable
class GeometrySetter(CommonPropertySetter):
    def __init__(self):
        super().__init__("geometry")

    def set_property(self, widget, property_value):
        widget.setGeometry(*property_value)


@plugin_loadable
class WindowTitleSetter(CommonPropertySetter):
    def __init__(self):
        super().__init__("windowTitle")

    def set_property(self, widget, property_value):
        widget.setWindowTitle(property_value)
