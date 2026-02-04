from ui import LayoutCreator, JSONWidgetLoader
from plugins import plugin_loadable
from PyQt5.QtWidgets import QLayout, QHBoxLayout, QVBoxLayout, QGridLayout, QStackedLayout
from PyQt5.QtCore import QTimer


@plugin_loadable
class QHBoxLayoutLayoutCreator(LayoutCreator):
    def __init__(self):
        super().__init__("QHBoxLayout")
    
    def create_layout(self, data, parent) -> QLayout:
        return QHBoxLayout()


@plugin_loadable
class QVBoxLayoutLayoutCreator(LayoutCreator):
    def __init__(self):
        super().__init__("QVBoxLayout")
    
    def create_layout(self, data, parent) -> QLayout:
        return QVBoxLayout()


@plugin_loadable
class QGridLayoutLayoutCreator(LayoutCreator):
    def __init__(self):
        super().__init__("QGridLayout")
    
    def create_layout(self, data, parent) -> QLayout:
        layout = QGridLayout()

        children = data["children"][:]
        data["children"] = []

        for child in children:
            widget = JSONWidgetLoader.init_widget(child, parent)
            layout.addWidget(
                widget, child["row"], child["col"],
                1 if "rowspawn" not in child.keys() else child["rowspan"],
                1 if "colspan" not in child.keys() else child["colspan"]
            )

        return layout


@plugin_loadable
class QStackedLayoutCreator(LayoutCreator):
    def __init__(self):
        super().__init__("QStackedLayout")
    
    def create_layout(self, data, parent) -> QLayout:
        layout = QStackedLayout()
        if "stackingMode" in data:
            layout.setStackingMode(QStackedLayout.StackingMode.StackAll if data["stackingMode"] == "all" else QStackedLayout.StackingMode.StackOne)
        if "currentIndex" in data:
            layout.setCurrentIndex(data["currentIndex"])
        return layout