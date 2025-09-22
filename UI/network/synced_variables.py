class SyncedVariables:
    variables = {}
    variable_listeners = {}

    @staticmethod
    def add_listener(variable, callback):
        if variable not in SyncedVariables.variable_listeners:
            SyncedVariables.variable_listeners[variable] = []
        SyncedVariables.variable_listeners[variable].append(callback)
