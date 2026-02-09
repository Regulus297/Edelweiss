from ui import WidgetCreator, JSONWidgetLoader, WidgetBinding
from interop import SyncableProperty
from plugins import plugin_loadable, load_dependencies
from PyQt5.QtWidgets import QWidget, QGridLayout, QLineEdit
import math


class FieldInfo:
    def __init__(self, name, displayName, fieldType, value):
        self.name = name
        self.displayName = displayName
        self.type = fieldType
        self.row = 0
        self.col = 0
        self.rowspan = 1
        self.colspan = 1
        self.data = {}
        self.value = value

    def __repr__(self):
        return f"{self.type} {self.name} = {self.value}, ({self.col}, {self.row}, {self.colspan}*{self.rowspan})"


# TODO: allow form data to be loaded through a different json file with a data key in the widget source
@plugin_loadable
@load_dependencies("Edelweiss:common_code")
class FormWidgetCreator(WidgetCreator):
    def __init__(self):
        super().__init__("Form")
        self.type_lookup = {int: "int", str: "str", bool: "bool", float: "float"}

    def create_widget(self, data, parent=None):
        fields = self._load_fields(data)
        self._apply_layout(data, fields)
        self._process_fields(data, fields)

        widget = JSONWidgetLoader.init_widget(self._generate_widget(fields), parent)

        
        return widget

    def _load_fields(self, data):
        field_data = {}
        fields = data["fields"]
        model = SyncableProperty(data["model"], False).get()
        for field in data["fields"]:
            info = FieldInfo(field, field, self.type_lookup.get(type(getattr(model, field).Value), "form"), f"{data["model"]}.{field}")
            field_data[field] = info

        return field_data

    def _process_fields(self, data, fields):
        fieldInfo = data.get("fieldInfo")
        if fieldInfo is None:
            return fields

        for fieldName, info in fieldInfo.items():
            field = fields[fieldName]
            field.type = info.get("type", field.type)
            field.data = info

    def _apply_layout(self, data, fields):
        layout = data.get("fieldLayout")
        if layout is None:
            self._apply_default_layout(fields)
            return
        self._apply_specific_layout(layout, fields)
    
    def _apply_default_layout(self, fields):
        i = 0
        for field in fields.values():
            field.col = 2 * (i % 2)
            field.row = i // 2
            field.rowspan = 1
            field.colspan = 2
            i += 1

    def _apply_specific_layout(self, layout, fields):
        nums = []
        for row in layout:
            nums.append(len(row))

        cols = 2 * self._lcm(nums)
        r = 0
        for row in layout:
            colsPerWidget = cols // len(row)
            col = 0
            for fieldName in row:
                field = fields[fieldName]
                field.col = colsPerWidget * col
                field.row = r
                field.rowspan = 1
                field.colspan = colsPerWidget
                col += 1
            r += 1

    def _lcm(self, nums):
        lcm = nums[0]
        for num in nums:
            gcd = math.gcd(lcm, num)
            lcm = lcm * num // gcd
        return lcm

    def _generate_widget(self, fields):
        children = []
        widget = {
            "type": "QWidget",
            "layout": {
                "type": "QGridLayout",
                "children": children
            }
        }

        for field in fields.values():
            children.append({
                "type": "QLabel",
                "text": field.displayName,
                "row": field.row,
                "col": field.col,
                "rowspan": field.rowspan,
                "colspan": 1
            })
            children.append(self._get_widget(field))

        return widget

    def _get_widget(self, field):
        widget = None
        if field.type == "str":
            widget = {
                "type": "QLineEdit",
                "bind": {
                    "text": field.value
                }
            }
        elif field.type == "list":
            widget = {
                "type": "QComboBox",
                "bind": {
                    "selected": field.value
                }
            }
            updateJSON(widget, field.data, "type")
        elif field.type == "bool":
            widget = {
                "type": "QCheckBox",
                "bind": {
                    "checked": field.value
                }
            }
        elif field.type == "int" or field.type == "float":
            widget = {
                "type": "QLineEdit",
                "dataType": field.type,
                "bind": {
                    "text": field.value
                }
            }
        elif field.type == "form":
            widget = {
                "type": "Form",
                "model": field.value,
                "id": "bordered"
            }
            updateJSON(widget, field.data, "type")
        elif field.type == "formList":
            widget = {
                "type": "FormList",
                "bind": {
                    "model": field.value
                }
            }
            updateJSON(widget, field.data, "type")


        widget["row"] = field.row
        widget["col"] = field.col + 1
        widget["rowspan"] = field.rowspan
        widget["colspan"] = field.colspan - 1
        return widget