from .variable_binding import VariableBinding


# TODO: allow changed to be None in which case add_item should be used for all items already in the list
class ListBinding:
    def __new__(cls, prop, changed, added, removed, add_item=None, remove_item=None):
        add_item = ListBinding.method_wrapper(add_item, lambda self, value: self.prop.get().Add(value))
        remove_item = ListBinding.method_wrapper(remove_item, lambda self, value: self.prop.get().Remove(value))
        return VariableBinding(prop, changed, None, {"ItemAdded": added, "ItemRemoved": removed}, {"add": add_item, "remove": remove_item})

    @staticmethod
    def method_wrapper(callback, default):
        def inner(self, value):
            if callback is None:
                default(self, value)
            callback(value)
        return inner
