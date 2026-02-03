from .variable_binding import VariableBinding


class ListBinding(VariableBinding):
    def __init__(self, prop, changed, added, removed):
        self.added = added
        self.removed = removed
        super().__init__(prop, changed, None)
        
    def bind(self):
        var = self.prop.get()
        var.ValueChanged += self.changed
        var.ItemAdded += self.added
        var.ItemRemoved += self.removed
        self.changed(var.Value)
