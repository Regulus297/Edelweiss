from .variable_binding import VariableBinding


class DictBinding:
    def __new__(cls, prop, clear, item_changed, item_added, item_removed, set_item=None, add_item=None, remove_item=None):
        set_item = DictBinding.method_wrapper(set_item, DictBinding._set_item_fallback)
        add_item = DictBinding.method_wrapper(add_item, lambda self, key, value: self.prop.raw_get().Add(key, value))
        remove_item = DictBinding.method_wrapper(remove_item, lambda self, key: self.prop.raw_get().Remove(key))
        changed = DictBinding.changed_wrapper(clear, item_added)
        return VariableBinding(prop, changed, None, {"ItemChanged": item_changed, "ItemAdded": item_added, "ItemRemoved": item_removed}, {"_set_item": set_item, "add": add_item, "remove": remove_item})

    @staticmethod
    def method_wrapper(callback, default):
        def inner(self, *args):
            if callback is None:
                default(self, *args)
                return
            callback(*args)

        return inner

    @staticmethod
    def changed_wrapper(clear, item_added):
        def inner(value):
            if clear is not None:
                clear()
            for item in value:
                item_added(item.Key, item.Value)

        return inner

    @staticmethod
    def _set_item_fallback(self, key, value):
        self.prop.raw_get()[key] = value
