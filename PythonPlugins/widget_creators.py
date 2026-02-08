from ui import WidgetCreator, WidgetBinding, WidgetMethod
from ui.widgets import ModifiableCombobox
from interop import ListBinding, VariableBinding, DictBinding
from plugins import plugin_loadable
from PyQt5.QtWidgets import QWidget, QLabel, QPushButton, QLineEdit, QComboBox, QCheckBox
from PyQt5.QtGui import QIntValidator, QDoubleValidator


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
        WidgetMethod.create(widget, widget.pressed, data, "click", None)
        return widget

@plugin_loadable
class QLineEditWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QLineEdit")

    def create_widget(self, data, parent=None):
        widget = QLineEdit(parent=parent)
        binding = WidgetBinding(data, "text", lambda x: widget.setText(str(x)), None)

        dataType = data.get("dataType")
        t = str
        if dataType == "int":
            widget.setValidator(QIntValidator(widget))
            t = int
        elif dataType == "float":
            widget.setValidator(QDoubleValidator(widget))
            t = float

        method = WidgetMethod.create(widget, widget.editingFinished, data, "edit", binding, {"text": lambda: t(widget.text())})
        if method is None:
            binding.bind(widget, widget.editingFinished, lambda b: b.set(t(widget.text())))

        return widget

@plugin_loadable
class QComboBoxWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QComboBox")

    def create_widget(self, data, parent=None):
        if data.get("modifiable"):
            return self._create_modifiable_combobox(data, parent)
        return self._create_combobox(data, parent)

    def _create_modifiable_combobox(self, data, parent):
        widget = ModifiableCombobox(WidgetBinding.get_value(data, "defaults"), parent)
        widget.canEditDefaults = data.get("canEditDefaults", True)
        params = {"text": widget.combobox.currentText, "index": widget.combobox.currentIndex}
        options_binding = WidgetBinding(data, "options", None, None, lambda prop, _, __: ListBinding(prop, widget.clear, widget.addItem, lambda item: self._remove_item(widget, item), widget.setItemText))
        change = WidgetMethod.create(widget, widget.itemChanged, data, "change", options_binding, params)
        add = WidgetMethod.create(widget, widget.itemAdded, data, "add", options_binding, params)
        remove = WidgetMethod.create(widget, widget.itemRemoved, data, "remove", options_binding, params)
        edit = WidgetMethod.create(widget, widget.itemEdited, data, "edit", options_binding, params)
        if add is None:
            options_binding.bind(widget, widget.itemAdded, lambda b: b.add(widget.combobox.currentText()))
        if remove is None:
            options_binding.bind(widget, widget.itemRemoved, lambda b: b.remove(widget.combobox.currentText()))
        if edit is None:
            options_binding.bind(widget, widget.itemEdited, lambda b: b.__setitem__(widget.combobox.currentIndex(), widget.combobox.currentText()))
        return widget
        
    def _create_combobox(self, data, parent):
        widget = QComboBox(parent=parent)
        binding = WidgetBinding(data, "options", widget.addItems, None, lambda prop, _, __: self._ctor(widget, prop))
        selected_binding = WidgetBinding(data, "selected", lambda text: self._set_text(widget, text), None)
        params = {"text": lambda: self._get_text(widget), "index": widget.currentIndex}
        method = WidgetMethod.create(widget, widget.currentIndexChanged, data, "change", binding, params)
        if method is None:
            selected_binding.bind(widget, widget.currentIndexChanged, lambda b: b.set(self._get_text(widget)), True)
        return widget

    def _ctor(self, widget, prop):
        prop_type = prop.get().GetType().Name
        if prop_type == "Dictionary`2":
            setattr(widget, "__keyed__", True)
            return DictBinding(prop, widget.clear, lambda key, value: self._replace_item(widget, key, value), widget.addItem, lambda key, _: self._remove_item(widget, key))
        return ListBinding(prop, widget.clear, widget.addItem, lambda item: self._remove_item(widget, item), widget.setItemText)

    def _remove_item(self, widget, key):
        i = widget.findText(key)
        widget.removeItem(i)

    def _replace_item(self, widget, key, value):
        i = widget.findText(key)
        widget.setItemData(i, value)

    def _get_text(self, widget):
        if getattr(widget, "__keyed__", False):
            return widget.currentData()
        return widget.currentText()

    def _set_text(self, widget, text):
        if widget.isEditable():
            widget.setCurrentText(text)
            return
        
        prev = widget.currentIndex()
        widget.setCurrentIndex(max(0, widget.findText(text)))

        if prev == widget.currentIndex():
            widget.currentIndexChanged.emit(0)


@plugin_loadable
class QCheckBoxWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QCheckBox")

    def create_widget(self, data, parent=None):
        widget = QCheckBox(parent)
        binding = WidgetBinding(data, "checked", widget.setChecked, None)
        method = WidgetMethod.create(widget, widget.toggled, data, "toggle", binding, {"checked": widget.isChecked})
        if method is None:
            binding.bind(widget, widget.toggled, lambda b: b.set(widget.isChecked()))
        return widget