from ui.json_widget_loader import JSONWidgetLoader, CommonPropertySetter
from plugins.plugin_loadable import plugin_loadable
from plugins.load_dependencies import load_dependencies


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
        widget.setStylesheet(property_value)