from .variable_binding import VariableBinding


class DictBinding:
    def __new__(cls, prop, changed, item_changed, item_added, item_removed, set_item=None, add_item=None, remove_item=None):
        set_item = DictBinding.method_wrapper(set_item, DictBinding._set_item_fallback)
        add_item = DictBinding.method_wrapper(add_item, lambda self, key, value: self.prop.get().Add(key, value))
        remove_item = DictBinding.key_method_wrapper(remove_item, lambda self, key: self.prop.get().Remove(key))
        return VariableBinding(prop, changed, None, {"ItemChanged": item_changed, "ItemAdded": item_added, "ItemRemoved": item_removed}, {"_set_item": set_item, "add": add_item, "remove": remove_item})

    @staticmethod
    def method_wrapper(callback, default):
        def inner(self, key, value):
            if callback is None:
                default(self, key, value)
            callback(key, value)

        return inner

    @staticmethod
    def key_method_wrapper(callback, default):
        def inner(self, key):
            if callback is None:
                default(self, key)
            callback(key)

        return inner


    @staticmethod
    def _set_item_fallback(self, key, value):
        self.prop.get()[key] = value
