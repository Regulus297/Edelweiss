import System as CSSystem

type_lookup = {int : CSSystem.Int32, object : CSSystem.Object, str : CSSystem.String, bool : CSSystem.Boolean}

# Convenience class to make creating actions easier: the default System.Action always shadows the generic version,
# so it's annoying to create a generic type
class _Action:
    def __new__(cls, callback):
        # non-generic action
        return CSSystem.Action(callback)

    def __class_getitem__(cls, item):
        if not isinstance(item, tuple):
            item = (item,)

        delegate_type = getattr(CSSystem, f"Action`{len(item)}")
        types = tuple(type_lookup[t] if t in type_lookup else t for t in item)

        return delegate_type[types]

# Made to prevent annoying red lines in code
class System:
    Int16 = CSSystem.Int16
    Int32 = CSSystem.Int32
    Int64 = CSSystem.Int64
    Int128 = CSSystem.Int128
    String = CSSystem.String
    Action = _Action
    Object = CSSystem.Object
    Boolean = CSSystem.Boolean
