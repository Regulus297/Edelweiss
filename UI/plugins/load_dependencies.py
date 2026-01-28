import os

def load_dependencies(*dependencies: list[str], load_types: bool = False):
    def inner(cls):
        cls.__dependencies__ = [dep.replace("/", os.sep) for dep in dependencies]
        cls.__load_types__ = load_types
        return cls
    return inner
