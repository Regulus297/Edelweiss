from types import MethodType

from .syncable_property import SyncableProperty

class VariableBinding:
    def __init__(self, prop, changed, modify, event_hooks=None, modifiers=None):
        """

        :param prop: The SyncableProperty that returns the BindableVariable
        :param changed: Callback when the SyncableProperty is changed
        :param modify: InteropMethod called when modifying the BindableVariable's value. If None, will use the setter for prop
        :param event_hooks: extra hooks to add to events defined by the BindableVariable
        :param modifiers: extra operations that can be performed on the BindableVariable
        """
        if event_hooks is None:
            event_hooks = {}
        if modifiers is None:
            modifiers = {}
        self.prop: SyncableProperty = prop
        self.changed = changed
        self.modify = modify
        self.event_hooks = event_hooks
        self.instance_type = VariableBinding
        for modifier_name, modifier in modifiers.items():
            setattr(self, modifier_name, MethodType(modifier, self))
        self.bind()


    def bind(self):
        var = self.prop.get()
        if self.prop.sync:
            var.ValueChanged += self.changed
            for name, callback in self.event_hooks.items():
                event = getattr(var, name)
                event += callback
        self.changed(var.Value)

    def set(self, value):
        if self.modify is not None:
            self.modify(value)
            return
        self.prop.set(value)

    # Added so _set_item actually works as overriding dunder methods manually does not work:
    def __setitem__(self, key, value):
        if hasattr(self, "_set_item"):
            self._set_item(key, value)
            return
        raise TypeError(f"This '{type(self)}' instance does not support item assignment")