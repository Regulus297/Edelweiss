from ui import WidgetCreator, JSONWidgetLoader, WidgetBinding, WidgetMethod
from ui.widgets import ModifiableCombobox, FormList
from plugins import plugin_loadable, load_dependencies
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
        WidgetBinding(data, "text", ValueChanged=widget.setText)
        return widget

@plugin_loadable
class QPushButtonWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QPushButton")

    def create_widget(self, data, parent=None):
        widget = QPushButton(parent=parent)
        binding = WidgetBinding(data, "text", ValueChanged=widget.setText)
        WidgetMethod.create(widget, widget.clicked, data, "click", binding)
        return widget

@plugin_loadable
class QLineEditWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QLineEdit")

    def create_widget(self, data, parent=None):
        widget = QLineEdit(parent=parent)
        binding = WidgetBinding(data, "text", ValueChanged=lambda v: widget.setText(str(v)))

        dataType = data.get("dataType")
        t = str
        if dataType == "int":
            widget.setValidator(QIntValidator(widget))
            t = int
        elif dataType == "float":
            widget.setValidator(QDoubleValidator(widget))
            t = float

        method = WidgetMethod.create(widget, widget.editingFinished, data, "edit", binding, {"text": lambda: t(widget.text)})
        if method is None:
            widget.editingFinished.connect(lambda: binding.prop.set(t(widget.text())))
        return widget


@plugin_loadable
class QComboBoxWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QComboBox")

    def create_widget(self, data, parent=None):
        if data.get("modifiable"):
            return self._create_modifiable(data, parent)
        return self._create_combobox(data, parent)

    def _create_combobox(self, data, parent):
        widget = QComboBox(parent=parent)
        options_binding = WidgetBinding(data, "options", ItemAdded=widget.addItem)
        setattr(widget, "__keyed__", options_binding.is_dict)
        selected_binding = WidgetBinding(data, "selected", ValueChanged=[lambda text: self._set_text(widget, text)])
        change = WidgetMethod.create(widget, widget.currentIndexChanged, data, "change", selected_binding, {"text": lambda: self._get_text(widget), "index": widget.currentIndex})
        if change is None:
            WidgetBinding.bind(widget.currentIndexChanged, lambda _: selected_binding.prop.set(self._get_text(widget)), pair=selected_binding.prop.ValueChanged, call_args=(None,))
        return widget

    def _set_text(self, widget, text):
        if widget.isEditable():
            widget.setCurrentText(text)
            return

        prev = widget.currentIndex()
        widget.setCurrentIndex(max(0, widget.findText(text)))

        if prev == widget.currentIndex():
            widget.currentIndexChanged.emit(0)

    def _get_text(self, widget):
        if getattr(widget, "__keyed__", False):
            return widget.currentData()
        return widget.currentText()

    def _create_modifiable(self, data, parent):
        widget = ModifiableCombobox(WidgetBinding.get_value(data, "defaults"), parent)
        widget.canEditDefaults = data.get("canEditDefaults", True)
        params = {"text": widget.combobox.currentText, "index": widget.combobox.currentIndex}
        options_binding = WidgetBinding(data, "options", ItemAdded=widget.addItem, ItemRemoved=lambda _, item: self._remove_item(widget, item), ItemChanged=widget.setItemText)
        options_binding.clear = widget.clear
        change = WidgetMethod.create(widget, widget.itemChanged, data, "change", options_binding, params)
        add = WidgetMethod.create(widget, widget.itemAdded, data, "add", options_binding, params)
        remove = WidgetMethod.create(widget, widget.itemRemoved, data, "removed", options_binding, params)
        edit = WidgetMethod.create(widget, widget.itemEdited, data, "edit", options_binding, params)
        if add is None:
            WidgetBinding.bind(widget.itemAdded, lambda v: options_binding.prop.add(v), pair=options_binding.prop.ItemAdded)
        if remove is None:
            WidgetBinding.bind(widget.itemRemoved, lambda v: options_binding.prop.remove(v), pair=options_binding.prop.ItemRemoved)
        if edit is None:
            WidgetBinding.bind(widget.itemEdited, lambda _, v: options_binding.prop.set_item(widget.combobox.currentIndex(), v), pair=options_binding.prop.ItemChanged)
        return widget
    

@plugin_loadable
class QCheckBoxWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QCheckBox")

    def create_widget(self, data, parent=None):
        widget = QCheckBox(parent)
        binding = WidgetBinding(data, "checked", ValueChanged=widget.setChecked)
        method = WidgetMethod.create(widget, widget.toggled, data, "toggle", binding, {"checked": widget.isChecked})
        if method is None:
            WidgetBinding.bind(widget.toggled, lambda _: binding.prop.set(widget.isChecked()), pair=binding.prop.ValueChanged)
        return widget

# TODO: fix this creator to not crash on model change
# @load_dependencies("Edelweiss:common_code")
# @plugin_loadable
# class FormListWidgetCreator(WidgetCreator):
#     def __init__(self):
#         super().__init__("FormList")

#     def create_widget(self, data, parent=None):
#         json = data["form"]
#         json["type"] = "Form"
#         model = data["bind"]["model"]

#         widget = FormList(lambda i: self._generate_widget(json, i, model), parent)
#         binding = WidgetBinding(data, "model", ItemAdded=lambda _: widget.new_row(), ItemRemoved=lambda index, _: widget.remove_row(widget.rows[index]))
#         binding.prop.clear = lambda: widget.clear(True)
#         WidgetBinding.bind(widget.itemAdded, lambda: binding.prop.add(None), pair=binding.prop.ItemAdded)
#         WidgetBinding.bind(widget.itemRemoved, lambda index: binding.prop.remove(binding.prop.get()[index]), pair=binding.prop.ItemRemoved)
#         return widget

#     def _generate_widget(self, json, index, model):
#         copied = copyJSON(json)
#         copied["model"] = f"{model}.@{index}"
#         return JSONWidgetLoader.init_widget(copied)