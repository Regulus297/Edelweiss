from PyQt5.QtCore import Qt
from PyQt5.QtWidgets import QWidget, QLayout, QSplitter, QPushButton, QSizePolicy


class JSONWidgetLoader:
    widget_creators = {}
    layout_creators = {}
    common_property_setters = {}

    @staticmethod
    def init_widget(data, parent=None) -> QWidget:
        widget_type = data["type"]
        if widget_type not in JSONWidgetLoader.widget_creators.keys():
            print(f"Unsupported widget type: {widget_type}")
            return QWidget(parent)

        widget: QWidget = JSONWidgetLoader.widget_creators[widget_type].create_widget(data, parent)
        setattr(widget, "__json_data__", data)
        JSONWidgetLoader.set_common_widget_props(widget, data)

        if "layout" in data.keys():
            layout_data = data["layout"]
            if layout_data["type"] not in JSONWidgetLoader.layout_creators.keys():
                print(f"Unsupported layout type: {layout_data['type']}")
                return widget

            layout: QLayout = JSONWidgetLoader.layout_creators[layout_data["type"]].create_layout(layout_data)
            widget.setLayout(layout)
            for child in layout_data["children"]:
                layout.addWidget(JSONWidgetLoader.init_widget(child, widget))

        return widget

    @staticmethod
    def set_common_widget_props(widget: QWidget, data: dict):
        for key, value in data.items():
            if key in JSONWidgetLoader.common_property_setters.keys():
                JSONWidgetLoader.common_property_setters[key].set_property(widget, value)


class WidgetCreator:
    def __init__(self, name):
        JSONWidgetLoader.widget_creators[name] = self

    def create_widget(self, data, parent=None) -> QWidget:
        ...

class LayoutCreator:
    def __init__(self, name):
        JSONWidgetLoader.layout_creators[name] = self

    def create_layout(self, data) -> QLayout:
        ...

class CommonPropertySetter:
    def __init__(self, prop):
        JSONWidgetLoader.common_property_setters[prop] = self

    def set_property(self, widget, property_value):
        ...