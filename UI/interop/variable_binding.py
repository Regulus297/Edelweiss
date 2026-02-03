from .syncable_property import SyncableProperty


class VariableBinding:
    def __init__(self, prop, changed, modify):
        """

        :param prop The SyncableProperty that returns the BindableVariable
        :param changed Callback when the SyncableProperty is changed
        :param modify InteropMethod called when modifying the BindableVariable's value. If None, will use the setter for prop
        """
        self.prop: SyncableProperty = prop
        self.changed = changed
        self.modify = modify
        self.bind()

    def bind(self):
        var = self.prop.get()
        var.ValueChanged += self.changed
        self.changed(var.Value)

    def set(self, value):
        if self.modify is not None:
            self.modify(value)
            return
        self.prop.set(value)
