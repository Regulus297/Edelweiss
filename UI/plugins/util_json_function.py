class UtilJsonFunction:
    functions = {}

    def __init__(self, name):
        UtilJsonFunction.functions[name] = self.call

    def call(self, *args) -> object:
        ...