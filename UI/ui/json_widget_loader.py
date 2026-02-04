from PyQt5.QtWidgets import QWidget, QLayout, QSizePolicy, QSpacerItem


# TODO: add a function to LayoutCreator to customise widget addition so QGridLayout doesn't need to modify the JSON
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

            JSONWidgetLoader.init_layout(layout_data, widget)

        return widget

    @staticmethod
    def init_layout(data, parent=None):
        layout: QLayout = JSONWidgetLoader.layout_creators[data["type"]].create_layout(data, parent)
        parent.setLayout(layout)
        JSONWidgetLoader.set_common_layout_props(layout, data)

        if "children" in data:
            for child in data["children"]:
                if "isLayout" in child and child["isLayout"]:
                    layout.addItem(JSONWidgetLoader.init_layout(child, parent))
                elif "isSpacer" in child and child["isSpacer"]:
                    layout.addItem(QSpacerItem(0, 0, QSizePolicy.Expanding, QSizePolicy.Expanding))
                else:
                    layout.addWidget(JSONWidgetLoader.init_widget(child, parent))
                if "alignment" in child:
                    layout.itemAt(layout.count() - 1).setAlignment(child["alignment"])

        return layout

    @staticmethod
    def set_common_widget_props(widget: QWidget, data: dict):
        for key, value in data.items():
            if key in JSONWidgetLoader.common_property_setters.keys():
                JSONWidgetLoader.common_property_setters[key].set_property(widget, value)

    @staticmethod
    def set_common_layout_props(layout: QLayout, data: dict):
        for key, value in data.items():
            if key in JSONWidgetLoader.common_property_setters.keys():
                JSONWidgetLoader.common_property_setters[key].set_layout_property(layout, value)


class WidgetCreator:
    def __init__(self, name):
        JSONWidgetLoader.widget_creators[name] = self

    def create_widget(self, data, parent=None) -> QWidget:
        ...

    def refresh_widget(self, widget):
        ...

class LayoutCreator:
    def __init__(self, name):
        JSONWidgetLoader.layout_creators[name] = self

    def create_layout(self, data, parent) -> QLayout:
        ...

class CommonPropertySetter:
    def __init__(self, prop):
        JSONWidgetLoader.common_property_setters[prop] = self

    def set_property(self, widget, property_value):
        ...

    def set_layout_property(self, layout, property_value):
        ...