from .variable_binding import VariableBinding


class DictBinding(VariableBinding):
    def __init__(self, prop, changed, item_changed, item_added, item_removed):
        self.item_changed = item_changed
        self.item_added = item_added
        self.item_removed = item_removed
        super().__init__(prop, changed, None)

    def bind(self):
        var = self.prop.get()
        var.ItemChanged += self.item_changed
        var.ItemAdded += self.item_added
        var.ItemRemoved += self.item_removed
        var.ValueChanged += self.changed
        self.changed(var.Value)