from ui import WidgetCreator, JSONWidgetLoader, WidgetBinding, WidgetMethod, LocalizedBinding
from ui.widgets import ModifiableCombobox, FormList, FileLineEdit
from plugins import plugin_loadable, load_dependencies
from utils import Enum
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
        LocalizedBinding(widget, data, "text", ValueChanged=widget.setText)
        return widget

@plugin_loadable
class QPushButtonWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QPushButton")

    def create_widget(self, data, parent=None):
        widget = QPushButton(parent=parent)
        binding = LocalizedBinding(widget, data, "text", ValueChanged=widget.setText)
        WidgetMethod.create(widget, widget.clicked, data, "click")
        return widget

@plugin_loadable
class QLineEditWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QLineEdit")

    def create_widget(self, data, parent=None):
        dataType = data.get("dataType")
        if dataType == "file":
            widget = FileLineEdit("", data.get("filter"), data.get("directory", False), parent)
            binding = WidgetBinding(widget, data, "text", ValueChanged=lambda v: widget.setText(str(v)))
            LocalizedBinding(widget, data, "title", ValueChanged=widget.setTitle)
            method = WidgetMethod.create(widget, widget.editingFinished, data, "edit", {"text": lambda: t(widget.text)})
            if method is None:
                widget.editingFinished.connect(lambda: binding.prop.set(widget.text()))
            return widget

        widget = QLineEdit(parent=parent)       
        t = str
        set_t = str
        if dataType == "int":
            widget.setValidator(QIntValidator(widget))
            t = int
        elif dataType == "float":
            validator = QDoubleValidator(widget)
            widget.setValidator(validator)
            set_t = lambda v: f"{v:.6f}".rstrip('0').rstrip('.')
            t = float

        binding = WidgetBinding(widget, data, "text", ValueChanged=lambda v: widget.setText(set_t(v)))

 
        method = WidgetMethod.create(widget, widget.editingFinished, data, "edit", {"text": lambda: t(widget.text)})
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
        options_binding = WidgetBinding(widget, data, "options", ItemAdded=widget.addItem)
        setattr(widget, "__keyed__", options_binding.is_dict)
        selected_binding = WidgetBinding(widget, data, "selected")
        if options_binding.prop is None:
            enumType = type(selected_binding.prop.get())
            if Enum.isEnum(enumType):
                setattr(widget, "__enum__", {})
                for value in Enum.getValues(enumType):
                    widget.addItem(value.ToString())
                    widget.__enum__[value.ToString()] = value
        
        selected_binding.prop.add_subscribers(ValueChanged=[lambda text: self._set_text(widget, text)])
        widget.setEditable(data.get("editable", False))

        change = WidgetMethod.create(widget, widget.currentIndexChanged, data, "change", {"text": lambda: self._get_text(widget), "index": widget.currentIndex})
        if change is None and selected_binding.prop is not None:
            WidgetBinding.bind(widget.currentIndexChanged, lambda _: selected_binding.prop.set(self._get_text(widget)), pair=selected_binding.prop.ValueChanged, call_args=(None,))
        return widget

    def _set_text(self, widget, text):
        if widget.isEditable():
            widget.setCurrentText(text)
            return
        
        if widget.__bindings__[1].prop is not None:
            widget.__bindings__[1].prop.ValueChanged.queue(lambda: self._set_index(widget, text))
    
    def _set_index(self, widget, text):
        prev = widget.currentIndex()
        if hasattr(widget, "__enum__"):
            widget.setCurrentIndex(max(0, widget.findText(text.ToString())))
        elif getattr(widget, "__keyed__", False):
            widget.setCurrentIndex(max(0, widget.findData(text)))
        else:
            widget.setCurrentIndex(max(0, widget.findText(text)))

        if prev == widget.currentIndex():
            widget.currentIndexChanged.emit(prev)

    def _get_text(self, widget):
        if hasattr(widget, "__enum__"):
            return widget.__enum__[widget.currentText()]
        if getattr(widget, "__keyed__", False):
            return widget.currentData()
        return widget.currentText()

    def _create_modifiable(self, data, parent):
        widget = ModifiableCombobox(WidgetBinding.get_value(data, "defaults"), parent)
        widget.canEditDefaults = data.get("canEditDefaults", True)
        params = {"text": widget.combobox.currentText, "index": widget.combobox.currentIndex}
        options_binding = WidgetBinding(widget, data, "options", ItemAdded=widget.addItem, ItemRemoved=lambda _, item: self._remove_item(widget, item), ItemChanged=widget.setItemText)
        options_binding.clear = widget.clear
        selected_binding = WidgetBinding(widget, data, "selected", ValueChanged=[lambda text: self._set_text(widget.combobox, text)])
        change = WidgetMethod.create(widget, widget.itemChanged, data, "change", params)
        add = WidgetMethod.create(widget, widget.itemAdded, data, "add", params)
        remove = WidgetMethod.create(widget, widget.itemRemoved, data, "remove", params)
        edit = WidgetMethod.create(widget, widget.itemEdited, data, "edit", params)
        if add is None:
            WidgetBinding.bind(widget.itemAdded, lambda v: options_binding.prop.add(v), pair=options_binding.prop.ItemAdded)
        if remove is None:
            WidgetBinding.bind(widget.itemRemoved, lambda v: options_binding.prop.remove(v), pair=options_binding.prop.ItemRemoved)
        if edit is None:
            WidgetBinding.bind(widget.itemEdited, lambda _, v: options_binding.prop.set_item(widget.combobox.currentIndex(), v), pair=options_binding.prop.ItemChanged)
        if change is None:
            WidgetBinding.bind(widget.itemChanged, lambda v: selected_binding.prop.set(v), pair=selected_binding.prop.ValueChanged)
        return widget
    

@plugin_loadable
class QCheckBoxWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("QCheckBox")

    def create_widget(self, data, parent=None):
        widget = QCheckBox(parent)
        binding = WidgetBinding(widget, data, "checked", ValueChanged=widget.setChecked)
        method = WidgetMethod.create(widget, widget.toggled, data, "toggle", {"checked": widget.isChecked})
        if method is None:
            WidgetBinding.bind(widget.toggled, lambda _: binding.prop.set(widget.isChecked()), pair=binding.prop.ValueChanged)
        return widget


@load_dependencies("Edelweiss:common_code")
@plugin_loadable
class FormListWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("FormList")

    def create_widget(self, data, parent=None):
        json = data["form"]
        json["type"] = "Form"
        model = data["bind"]["model"]

        widget = FormList(lambda i: self._generate_widget(json, i, model), parent)
        binding = WidgetBinding(widget, data, "model", ItemAdded=lambda _: widget.new_row(), ItemRemoved=lambda index, _: widget.remove_row(widget.rows[index]))
        binding.prop.clear = lambda: widget.clear(True)
        WidgetBinding.bind(widget.itemAdded, lambda: binding.prop.add(None), pair=binding.prop.ItemAdded)
        WidgetBinding.bind(widget.itemRemoved, lambda index: binding.prop.remove(binding.prop.get()[index]), pair=binding.prop.ItemRemoved)
        return widget

    def _generate_widget(self, json, index, model):
        copied = copyJSON(json)
        copied["model"] = f"{model}.@{index}"
        return JSONWidgetLoader.init_widget(copied)
