def load_dependencies(*dependencies):
    def inner(cls):
        cls.__dependencies__ = dependencies
        return cls
    return inner