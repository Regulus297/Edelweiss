from PyQt5.QtCore import Qt
from PyQt5.QtWidgets import QWidget, QLayout, QSplitter, QPushButton, QSizePolicy


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
        if "id" in data.keys():
            widget.setObjectName(data["id"])
        if "sizePolicyX" in data.keys():
            policy = widget.sizePolicy()
            policy.setHorizontalPolicy(JSONWidgetLoader.get_size_policy(data["sizePolicyX"]))
            widget.setSizePolicy(policy)
        if "sizePolicyY" in data.keys():
            policy = widget.sizePolicy()
            policy.setHorizontalPolicy(JSONWidgetLoader.get_size_policy(data["sizePolicyY"]))
            widget.setSizePolicy(policy)
        if "stylesheet" in data.keys():
            widget.setStyleSheet(data["stylesheet"])

    @staticmethod
    def get_size_policy(policy: str) -> QSizePolicy:
        if policy == "Expanding":
            return QSizePolicy.Expanding
        elif policy == "Preferred":
            return QSizePolicy.Preferred

        return  QSizePolicy.Fixed

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