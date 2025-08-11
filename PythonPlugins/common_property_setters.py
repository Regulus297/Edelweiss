from ui import JSONWidgetLoader, CommonPropertySetter, MappingWindow
from plugins import plugin_loadable, load_dependencies
from PyQt5.QtWidgets import QPushButton, QApplication


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
        super().__init__("style")

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


@load_dependencies("common_code.py")
@plugin_loadable
class SpecialTypeSetter(CommonPropertySetter):
    def __init__(self):
        super().__init__("specialType")

    def set_property(self, widget, property_value):
        if property_value == "submit" and isinstance(widget, QPushButton):
            CommonVars.found_submit_button = widget

@plugin_loadable
class MaxHeightSetter(CommonPropertySetter):
    def __init__(self):
        super().__init__("maxHeight")

    def set_property(self, widget, property_value):
        if isinstance(property_value, float):
            property_value = int(property_value * MappingWindow.instance.height())
        widget.setMaximumHeight(property_value)


@load_dependencies("common_code.py")
@plugin_loadable
class AlignmentSetter(CommonPropertySetter):
    def __init__(self):
        super().__init__("alignment")

    def set_property(self, widget, property_value):
        item = get_layout_item(widget)
        if item is not None:
            item.setAlignment(value(property_value))

    def set_layout_property(self, layout, property_value):
        layout.setAlignment(value(property_value))


@plugin_loadable
class ContentsMarginsSetter(CommonPropertySetter):
    def __init__(self):
        super().__init__("contentsMargins")

    def set_layout_property(self, layout, property_value):
        layout.setContentsMargins(*property_value)