from .variable_binding import VariableBinding


# This could be a method, but semantically it makes sense for it to be a class
class ListBinding:
    def __new__(cls, prop, changed, added, removed, item_changed, add_item=None, remove_item=None, set_item=None):
        add_item = ListBinding.method_wrapper(add_item, lambda self, value: self.prop.get().Add(value))
        remove_item = ListBinding.method_wrapper(remove_item, lambda self, value: self.prop.get().Remove(value))
        set_item = ListBinding.method_wrapper(set_item, ListBinding._set_item_fallback)
        changed = ListBinding.changed_wrapper(changed, added)
        return VariableBinding(prop, changed, None, {"ItemAdded": added, "ItemRemoved": removed, "ItemChanged": item_changed}, {"add": add_item, "remove": remove_item, "_set_item": set_item})

    @staticmethod
    def method_wrapper(callback, default):
        def inner(self, *args):
            if callback is None:
                default(self, *args)
                return
            callback(*args)
        return inner

    @staticmethod
    def changed_wrapper(callback, added):
        def inner(value):
            if callback is None:
                for item in value:
                    added(item)
                return
            callback(value)

        return inner

    @staticmethod
    def _set_item_fallback(self, i, value):
        self.prop.get()[i] = value