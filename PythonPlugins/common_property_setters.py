from ui import JSONWidgetLoader, CommonPropertySetter, MainWindow, LocalizedBinding
from plugins import plugin_loadable, load_dependencies
from utils import UI
from PyQt5.QtWidgets import QPushButton, QApplication


@plugin_loadable
class IDPropertySetter(CommonPropertySetter):
    def __init__(self):
        super().__init__("id")

    def set_property(self, widget, property_value):
        widget.setObjectName(property_value)


@load_dependencies("Edelweiss:common_code")
@plugin_loadable
class SizePolicyXSetter(CommonPropertySetter):
    def __init__(self):
        super().__init__("sizePolicyX")

    def set_property(self, widget, property_value):
        policy = widget.sizePolicy()
        policy.setHorizontalPolicy(get_size_policy(property_value))
        widget.setSizePolicy(policy)


@load_dependencies("Edelweiss:common_code")
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
        super().__init__("style")

    def set_property(self, widget, property_value):
        widget.setStyleSheet(property_value)


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


@load_dependencies("Edelweiss:common_code")
@plugin_loadable
class SpecialTypeSetter(CommonPropertySetter):
    def __init__(self):
        super().__init__("specialType")

    def set_property(self, widget, property_value):
        if property_value == "submit" and isinstance(widget, QPushButton):
            widget.clicked.connect(lambda: UI.close(widget.parent()))

@plugin_loadable
class MaxHeightSetter(CommonPropertySetter):
    def __init__(self):
        super().__init__("maxHeight")

    def set_property(self, widget, property_value):
        if isinstance(property_value, float):
            property_value = int(property_value * MainWindow.instance.height())
        widget.setMaximumHeight(property_value)


@plugin_loadable
class ContentsMarginsSetter(CommonPropertySetter):
    def __init__(self):
        super().__init__("contentsMargins")

    def set_layout_property(self, layout, property_value):
        layout.setContentsMargins(*property_value)


@plugin_loadable
class TooltipSetter(CommonPropertySetter):
    def __init__(self):
        super().__init__("tooltip")

    def set_property(self, widget, property_value):
        binding = LocalizedBinding(widget, {"tooltip": property_value}, "tooltip", ValueChanged=[widget.setToolTip])
        binding.default = None
        