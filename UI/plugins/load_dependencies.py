import os

def load_dependencies(*dependencies: list[str]):
    def inner(cls):
        cls.__dependencies__ = [dep.replace("/", os.sep) for dep in dependencies]
        return cls
    return inner