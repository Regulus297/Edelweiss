from ui import WidgetCreator, WidgetBinding, WidgetMethod
from plugins import plugin_loadable
from PyQt5.QtWidgets import QWidget, QLabel, QPushButton, QLineEdit


@plugin_loadable
class QWidgetWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QWidget")

    def create_widget(self, data, parent=None):
        return QWidget(parent)


@plugin_loadable
class QLabelWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QLabel")

    def create_widget(self, data, parent=None):
        widget = QLabel(parent=parent)
        WidgetBinding(data, "text", widget.setText, None)
        return widget

@plugin_loadable
class QPushButtonWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QPushButton")

    def create_widget(self, data, parent=None):
        widget = QPushButton(parent=parent)
        WidgetBinding(data, "text", widget.setText, None)
        WidgetMethod.create(widget, widget.pressed, data, "click")
        return widget

@plugin_loadable
class QLineEditWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QLineEdit")

    def create_widget(self, data, parent=None):
        widget = QLineEdit(parent=parent)
        binding = WidgetBinding(data, "text", widget.setText, None)
        method = WidgetMethod.create(widget, widget.editingFinished, data, "edit", {"text": widget.text})
        if method is None:
            binding.bind(widget, widget.editingFinished, lambda prop: prop.set(widget.text()))
        return widget
