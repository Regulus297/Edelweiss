from PyQt5.QtCore import Qt
from PyQt5.QtWidgets import QWidget, QLayout, QSplitter


class JSONWidgetLoader:
    widget_creators = {}
    layout_creators = {}

    @staticmethod
    def init_widget(data, parent=None) -> QWidget:
        widget_type = data["type"]
        if widget_type not in JSONWidgetLoader.widget_creators.keys():
            print(f"Unsupported widget type: {widget_type}")
            return QWidget(parent)

        widget: QWidget = JSONWidgetLoader.widget_creators[widget_type].create_widget(data, parent)

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